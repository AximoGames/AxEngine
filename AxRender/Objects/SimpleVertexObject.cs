// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Aximo.Render
{

    public class SimpleVertexObject : RenderableObject, IShadowObject, IReloadable, ILightTarget, IScaleRotate,
        IForwardRenderable, IDeferredRenderable
    {

        public Camera Camera => Context.Camera;
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1.0f);
        public Vector3 Rotate { get; set; }

        public bool RenderShadow { get; set; } = true;

        public Matrix4 PositionMatrix = RenderContext.Current.WorldPositionMatrix;

        public Matrix4 GetModelMatrix()
        {
            return Matrix4.CreateScale(Scale)
            * Matrix4.CreateRotationX(Rotate.X * (MathF.PI * 2))
            * Matrix4.CreateRotationY(Rotate.Y * (MathF.PI * 2))
            * Matrix4.CreateRotationZ(Rotate.Z * (MathF.PI * 2))
            * Matrix4.CreateTranslation((new Vector4(Position, 1.0f) * PositionMatrix).Xyz);
        }

        private Material _MaterialTmp;
        public Material Material
        {
            get
            {
                return Mesh?.Material;
            }
            set
            {
                if (Mesh == null)
                {
                    _MaterialTmp = value;
                    return;
                }
                Mesh.Material = value;
            }
        }

        public bool Debug;

        private Mesh Mesh;

        private List<VertexArrayObjectMaterial> vaoList = new List<VertexArrayObjectMaterial>();

        class VertexArrayObjectMaterial
        {
            public VertexArrayObject vao;
            public Material material;
        }

        public IRenderPipeline PrimaryRenderPipeline;

        public override void Init()
        {
            UsePipeline<PointShadowRenderPipeline>();
            UsePipeline<DirectionalShadowRenderPipeline>();

            if (PrimaryRenderPipeline == null)
                PrimaryRenderPipeline = Context.PrimaryRenderPipeline;

            UsePipeline(PrimaryRenderPipeline);

            if (_MaterialTmp != null)
            {
                Mesh.Material = _MaterialTmp;
                _MaterialTmp = null;
            }

            foreach (var m in Mesh.Materials)
            {
                m.CreateShaders();

                var vao = new VertexArrayObject(Mesh.MeshData.BindLayoutToShader(m.Shader));
                vao.SetData(Mesh.MeshData);
                vaoList.Add(new VertexArrayObjectMaterial
                {
                    vao = vao,
                    material = m,
                });
            }
        }

        public void SetVertices(Mesh mesh)
        {
            Mesh = mesh;
        }

        public void OnForwardRender()
        {
            foreach (var mat in vaoList)
            {
                var shader = mat.material.Shader;
                mat.vao.Bind();

                if (mat.material.txt0 != null)
                    mat.material.txt0.Bind(TextureUnit.Texture0);
                if (mat.material.txt1 != null)
                    mat.material.txt1.Bind(TextureUnit.Texture1);
                Context.GetPipeline<DirectionalShadowRenderPipeline>().FrameBuffer.GetDestinationTexture().Bind(TextureUnit.Texture2);

                shader.Bind();

                shader.SetMatrix4("model", GetModelMatrix());
                shader.SetMatrix4("view", Camera.ViewMatrix);
                shader.SetMatrix4("projection", Camera.ProjectionMatrix);

                shader.SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);

                shader.SetInt("shadowMap", 2);

                shader.SetMaterial("material", mat.material);

                ApplyShaderParams(shader);

                //_Shader.SetVector3("light.position", GetShadowLight().Position);
                //_Shader.SetVector3("light.color", new Vector3(0.5f, 0.5f, 0.5f));
                shader.SetVector3("viewPos", Camera.Position);

                //var shadowCamera = GetShadowLight().LightCamera;
                shader.SetFloat("far_plane", 25f);
                Context.GetPipeline<PointShadowRenderPipeline>().FrameBuffer.GetDestinationTexture().Bind(TextureUnit.Texture3);
                shader.SetInt("depthMap", 3);

                shader.BindBlock("lightsArray", Context.LightBinding);
                shader.SetInt("lightCount", Lights.Count);

                mat.vao.Draw();
            }
        }

        public void OnDeferredRender()
        {
            var pipe = Context.GetPipeline<DeferredRenderPipeline>();
            if (pipe.Pass == DeferredPass.Pass1)
            {

                foreach (var mat in vaoList)
                {
                    var defGeometryShader = mat.material.DefGeometryShader;
                    mat.vao.Bind();

                    if (mat.material.txt0 != null)
                        mat.material.txt0.Bind(TextureUnit.Texture0);
                    if (mat.material.txt1 != null)
                        mat.material.txt1.Bind(TextureUnit.Texture1);

                    defGeometryShader.Bind();

                    defGeometryShader.SetMaterial("material", mat.material);

                    defGeometryShader.SetMatrix4("model", GetModelMatrix());
                    defGeometryShader.SetMatrix4("view", Camera.ViewMatrix);
                    defGeometryShader.SetMatrix4("projection", Camera.ProjectionMatrix);

                    mat.vao.Draw();
                }
            }

            if (pipe.Pass == DeferredPass.Pass2)
            {
            }
        }

        private Matrix4 lightSpaceMatrix;

        // Because of shared variables in the shaders, only one position is possible yet
        // private int TMP_LIGHT_IDX = 0; // 1= moving, 0=static light

        // private ILightObject GetCubeShadowLight()
        // {
        //     return Lights[TMP_LIGHT_IDX];
        // }

        // private ILightObject GetShadowLight()
        // {
        //     return Lights[TMP_LIGHT_IDX];
        // }

        public void OnRenderShadow()
        {
            foreach (var mat in vaoList)
            {
                mat.vao.Bind();
                var shadowShader = mat.material.ShadowShader;

                shadowShader.Bind();

                foreach (var light in Lights)
                {
                    var shadowCamera = light.LightCamera;

                    var lightProjection = shadowCamera.ProjectionMatrix;
                    var lightView = shadowCamera.ViewMatrix;
                    lightSpaceMatrix = lightView * lightProjection;

                    shadowShader.SetMatrix4("model", GetModelMatrix());
                    shadowShader.SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);
                    shadowShader.SetInt("shadowLayer", light.ShadowTextureIndex);

                    //GL.CullFace(CullFaceMode.Front);
                    mat.vao.Draw();
                    //GL.CullFace(CullFaceMode.Back);}
                }
            }
        }

        private List<Matrix4> CubeShadowsMatrices = new List<Matrix4>();

        public void OnRenderCubeShadow()
        {
            foreach (var mat in vaoList)
            {
                mat.vao.Bind();
                var cubeShadowShader = mat.material.CubeShadowShader;
                cubeShadowShader.Bind();

                foreach (var light in Lights)
                {
                    var shadowCamera = light.LightCamera;

                    CubeShadowsMatrices.Clear();

                    if (CUBE_MAP_SHADOW_ROTATED)
                    {
                        AddShadowCubeMatrix(shadowCamera, new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f));
                        AddShadowCubeMatrix(shadowCamera, new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f));
                        AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                        AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f));
                        AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f));
                        AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f));
                    }
                    else
                    {
                        AddShadowCubeMatrix(shadowCamera, new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                        AddShadowCubeMatrix(shadowCamera, new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                        AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f));
                        AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f));
                        AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                        AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
                    }

                    cubeShadowShader.SetMatrix4("model", GetModelMatrix());
                    for (var i = 0; i < CubeShadowsMatrices.Count; i++)
                        cubeShadowShader.SetMatrix4($"shadowMatrices[{i}]", CubeShadowsMatrices[i]);
                    cubeShadowShader.SetVector3("light.position", light.Position);
                    cubeShadowShader.SetFloat("far_plane", shadowCamera.FarPlane);
                    cubeShadowShader.SetInt("shadowLayer", light.ShadowTextureIndex);

                    mat.vao.Draw();
                }
            }
        }

        public const bool CUBE_MAP_SHADOW_ROTATED = true;

        private void AddShadowCubeMatrix(Camera camera, Vector3 direction, Vector3 up)
        {
            var proj = camera.ProjectionMatrix;
            var view = Matrix4.LookAt(camera.Position, camera.Position + direction, up);
            if (CUBE_MAP_SHADOW_ROTATED)
                CubeShadowsMatrices.Add(view * proj);
            else
                CubeShadowsMatrices.Add(view * proj * Matrix4.CreateScale(1, -1, 1));
        }

        public override void Free()
        {
        }

        public void OnReload()
        {
        }

        public List<ILightObject> Lights => Context.LightObjects;

    }

}
