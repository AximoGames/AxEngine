// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Aximo.Render.OpenGL;
using Aximo.Render.Pipelines;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aximo.Render.Objects
{
    public class SimpleVertexObject : PrimitiveObject, IShadowObject, IReloadable, ILightTarget, IScaleRotate,
        IForwardRenderable, IDeferredRenderable, IScreenRenderable, IBounds
    {
        public Camera Camera => Context.Camera;
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1.0f);
        public Quaternion Rotate { get; set; }

        public bool RenderShadow { get; set; } = true;

        public Matrix4 PositionMatrix = RenderContext.Current.WorldPositionMatrix;

        public Matrix4 GetModelMatrix()
        {
            if (PositionMatrix != Matrix4.Identity)
            {
                return PositionMatrix;
            }

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

        private InternalMesh Mesh;

        private List<VertexArrayObjectMaterial> vaoList = new List<VertexArrayObjectMaterial>();

        private class VertexArrayObjectMaterial
        {
            public VertexArrayObject Vao;
            public Material Material;
        }

        public IRenderPipeline PrimaryRenderPipeline;

        public override void Init()
        {
            if (Mesh == null)
                return;

            if (Mesh.Materials.Any(m => m.CastShadow))
            {
                UsePipeline<PointShadowRenderPipeline>();
                UsePipeline<DirectionalShadowRenderPipeline>();
            }

            if (PrimaryRenderPipeline == null)
                PrimaryRenderPipeline = Context.PrimaryRenderPipeline;

            if (_MaterialTmp != null)
            {
                Mesh.Material = _MaterialTmp;
                _MaterialTmp = null;
            }

            if (Mesh.Materials.All(m => m.RenderPipeline == null))
                UsePipeline(PrimaryRenderPipeline);

            foreach (var materialId in Mesh.MaterialIds)
            {
                var m = Mesh.GetMaterial(materialId);
                UsePipeline(m.RenderPipeline);
                m.CreateShaders();

                var data = Mesh.GetMeshData(materialId);
                var vao = new VertexArrayObject(data.BindLayoutToShader(m.Shader));
                vao.SetData(data);
                vaoList.Add(new VertexArrayObjectMaterial
                {
                    Vao = vao,
                    Material = m,
                });
            }
        }

        public void SetVertices(InternalMesh mesh)
        {
            Mesh = mesh;
        }

        public void OnForwardRender()
        {
            foreach (var mat in vaoList)
            {
                var shader = mat.Material.Shader;
                shader.Bind();
                mat.Vao.Bind();

                if (mat.Material.DiffuseMap != null)
                    mat.Material.DiffuseMap.Bind(TextureUnit.Texture0);
                if (mat.Material.SpecularMap != null)
                    mat.Material.SpecularMap.Bind(TextureUnit.Texture1);

                var model = GetModelMatrix();

                shader.SetMatrix4("Model", model);
                shader.SetMatrix4("View", Camera.ViewMatrix);
                shader.SetMatrix4("Projection", Camera.ProjectionMatrix);

                shader.SetMatrix4("LightSpaceMatrix", LightSpaceMatrix);

                shader.SetMaterial("material", mat.Material);

                ApplyShaderParams(shader);

                shader.SetVector3("ViewPos", Camera.Position);

                if (Renderer.Current.UseShadows)
                {
                    Context.GetPipeline<DirectionalShadowRenderPipeline>().FrameBuffer.GetDestinationTexture().Bind(TextureUnit.Texture2);
                    shader.SetInt("DirectionalShadowMap", 2);
                    Context.GetPipeline<PointShadowRenderPipeline>().FrameBuffer.GetDestinationTexture().Bind(TextureUnit.Texture3);
                    shader.SetInt("PointShadowMap", 3);
                }

                shader.BindBlock("LightsArray", Context.LightBinding);
                shader.SetInt("LightCount", Lights.Count);

                mat.Vao.Draw();
            }
        }

        public void OnScreenRender()
        {
            foreach (var mat in vaoList)
            {
                var shader = mat.Material.Shader;
                shader.Bind();

                mat.Vao.Bind();

                if (mat.Material.DiffuseMap != null)
                    mat.Material.DiffuseMap.Bind(TextureUnit.Texture0);

                shader.SetMatrix4("Model", GetModelMatrix());

                //GL.Disable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Front);
                mat.Vao.Draw();
                //GL.Enable(EnableCap.CullFace);
                GL.CullFace(CullFaceMode.Back);
            }
        }

        public void OnDeferredRender()
        {
            var pipe = Context.GetPipeline<DeferredRenderPipeline>();
            if (pipe.Pass == DeferredPass.Pass1)
            {
                foreach (var mat in vaoList)
                {
                    var defGeometryShader = mat.Material.DefGeometryShader;
                    defGeometryShader.Bind();

                    mat.Vao.Bind();

                    if (mat.Material.DiffuseMap != null)
                        mat.Material.DiffuseMap.Bind(TextureUnit.Texture0);
                    if (mat.Material.SpecularMap != null)
                        mat.Material.SpecularMap.Bind(TextureUnit.Texture1);

                    defGeometryShader.SetMaterial("material", mat.Material);

                    var model = GetModelMatrix();
                    defGeometryShader.SetMatrix4("Model", model);
                    //defGeometryShader.SetMatrix3("NormalMatrix", Matrix3.Transpose(Matrix3.Invert(new Matrix3(model))));
                    defGeometryShader.SetMatrix4("View", Camera.ViewMatrix);
                    defGeometryShader.SetMatrix4("Projection", Camera.ProjectionMatrix);

                    mat.Vao.Draw();
                }
            }

            if (pipe.Pass == DeferredPass.Pass2)
            {
            }
        }

        private Matrix4 LightSpaceMatrix;

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
            var lights = Lights.Where(l => l.LightType == LightType.Directional).ToList();
            if (lights.Count == 0)
                return;

            foreach (var mat in vaoList)
            {
                if (!mat.Material.CastShadow)
                    continue;

                mat.Vao.Bind();
                var shadowShader = mat.Material.ShadowShader;

                shadowShader.Bind();

                foreach (var light in lights)
                {
                    var shadowCamera = light.LightCamera;

                    var lightProjection = shadowCamera.ProjectionMatrix;
                    var lightView = shadowCamera.ViewMatrix;
                    LightSpaceMatrix = lightView * lightProjection;

                    shadowShader.SetMatrix4("Model", GetModelMatrix());
                    shadowShader.SetMatrix4("LightSpaceMatrix", LightSpaceMatrix);
                    shadowShader.SetInt("ShadowLayer", light.ShadowTextureIndex);

                    //GL.CullFace(CullFaceMode.Front);
                    //GL.Disable(EnableCap.CullFace);
                    mat.Vao.Draw();
                    //GL.Enable(EnableCap.CullFace);
                    //GL.CullFace(CullFaceMode.Back);}
                }
            }
        }

        private List<Matrix4> CubeShadowsMatrices = new List<Matrix4>();

        public void OnRenderCubeShadow()
        {
            var lights = Lights.Where(l => l.LightType == LightType.Point).ToList();
            if (lights.Count == 0)
                return;

            foreach (var mat in vaoList)
            {
                if (!mat.Material.CastShadow)
                    continue;

                mat.Vao.Bind();
                var cubeShadowShader = mat.Material.CubeShadowShader;
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

                    cubeShadowShader.SetMatrix4("Model", GetModelMatrix());
                    for (var i = 0; i < CubeShadowsMatrices.Count; i++)
                        cubeShadowShader.SetMatrix4($"ShadowMatrices[{i}]", CubeShadowsMatrices[i]);
                    cubeShadowShader.SetVector3("Light.Position", light.Position);
                    cubeShadowShader.SetFloat("Light.FarPlane", shadowCamera.FarPlane);
                    cubeShadowShader.SetInt("Light.ShadowLayer", light.ShadowTextureIndex);

                    //GL.Disable(EnableCap.CullFace);
                    mat.Vao.Draw();
                    //GL.Enable(EnableCap.CullFace);
                }
            }
        }

        private bool CUBE_MAP_SHADOW_ROTATED = true;

        private void AddShadowCubeMatrix(Camera camera, Vector3 direction, Vector3 up)
        {
            var proj = camera.ProjectionMatrix;
            var view = Matrix4.LookAt(camera.Position, camera.Position + direction, up);
            if (CUBE_MAP_SHADOW_ROTATED)
                CubeShadowsMatrices.Add(view * proj);
            else
                CubeShadowsMatrices.Add(view * proj * Matrix4.CreateScale(1, -1, 1));
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                Free();
            }

            base.Dispose(disposing);
        }

        public override void Free()
        {
            foreach (var itm in vaoList)
            {
                itm.Vao.Free();
            }
            vaoList.Clear();
        }

        public void OnReload()
        {
        }

        public List<ILightObject> Lights => Context.LightObjects;

        public Box3 WorldBounds { get; set; }
    }
}
