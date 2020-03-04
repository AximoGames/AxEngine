using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace AxEngine
{

    public abstract class RenderableObject : GameObject, IRenderableObject
    {
        public void OnRender()
        {
            if (Context.CurrentPipeline is ForwardRenderPipeline && this is IForwardRenderable m1)
                m1.OnForwardRender();
            else if (Context.CurrentPipeline is DeferredRenderPipeline && this is IDeferredRenderable m2)
                m2.OnDeferredRender();
            else if (Context.CurrentPipeline is DirectionalShadowRenderPipeline && this is IShadowObject m3)
                m3.OnRenderShadow();
            else if (Context.CurrentPipeline is PointShadowRenderPipeline && this is IShadowObject m4)
                m4.OnRenderCubeShadow();
        }
    }

    public class SimpleVertexObject : RenderableObject, IShadowObject, IReloadable, ILightTarget, IScaleRotate,
        IForwardRenderable, IDeferredRenderable
    {

        public Camera Camera => Context.Camera;
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; } = new Vector3(1.0f);
        public Vector3 Rotate { get; set; }

        public bool RenderShadow { get; set; } = true;

        public Material Material { get; set; } = Material.GetDefault();

        public Matrix4 PositionMatrix = RenderApplication.Current.WorldPositionMatrix;

        public Matrix4 GetModelMatrix()
        {
            return Matrix4.CreateScale(Scale)
            * Matrix4.CreateRotationX(Rotate.X)
            * Matrix4.CreateRotationY(Rotate.Y)
            * Matrix4.CreateRotationZ(Rotate.Z)
            * Matrix4.CreateTranslation((new Vector4(Position, 1.0f) * PositionMatrix).Xyz);
        }

        public bool Debug;

        private Shader _Shader;
        private Shader _DefGeometryShader;
        private Shader _ShadowShader;
        private Shader _CubeShadowShader;

        private float[] _vertices = DataHelper.Cube;

        private VertexArrayObject vao;

        private Texture txt0;
        private Texture txt1;

        public IRenderPipeline PrimaryRenderPipeline;

        public override void Init()
        {
            UsePipeline<PointShadowRenderPipeline>();
            UsePipeline<DirectionalShadowRenderPipeline>();

            if (PrimaryRenderPipeline == null)
                PrimaryRenderPipeline = Context.PrimaryRenderPipeline;

            UsePipeline(PrimaryRenderPipeline);

            _Shader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _DefGeometryShader = new Shader("Shaders/deferred-gbuffer.vert", "Shaders/deferred-gbuffer.frag");

            txt0 = new Texture(Material.DiffuseImagePath);
            txt1 = new Texture(Material.SpecularImagePath);

            _ShadowShader = new Shader("Shaders/shadow-directional.vert", "Shaders/shadow-directional.frag", "Shaders/shadow-directional.geom");
            _CubeShadowShader = new Shader("Shaders/shadow-cube.vert", "Shaders/shadow-cube.frag", "Shaders/shadow-cube.geom");

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aPos"), 3);
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aNormal"), 3);
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aTexCoords"), 2);

            vao = new VertexArrayObject(layout);
            vao.SetData(_vertices);
        }

        public void SetVertices(float[] vertices)
        {
            _vertices = vertices;
        }

        public void OnForwardRender()
        {
            vao.Use();

            txt0.Use(TextureUnit.Texture0);
            txt1.Use(TextureUnit.Texture1);
            Context.GetPipeline<DirectionalShadowRenderPipeline>().FrameBuffer.GetDestinationTexture().Use(TextureUnit.Texture2);

            _Shader.Use();

            _Shader.SetMatrix4("model", GetModelMatrix());
            _Shader.SetMatrix4("view", Camera.ViewMatrix);
            _Shader.SetMatrix4("projection", Camera.ProjectionMatrix);

            _Shader.SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);

            _Shader.SetInt("shadowMap", 2);

            _Shader.SetMaterial("material", Material);

            //_Shader.SetVector3("light.position", GetShadowLight().Position);
            //_Shader.SetVector3("light.color", new Vector3(0.5f, 0.5f, 0.5f));
            _Shader.SetVector3("viewPos", Camera.Position);

            //var shadowCamera = GetShadowLight().LightCamera;
            _Shader.SetFloat("far_plane", 25f);
            Context.GetPipeline<PointShadowRenderPipeline>().FrameBuffer.GetDestinationTexture().Use(TextureUnit.Texture3);
            _Shader.SetInt("depthMap", 3);

            _Shader.BindBlock("lightsArray", Context.LightBinding);
            _Shader.SetInt("lightCount", Lights.Count);

            vao.Draw();
        }

        public void OnDeferredRender()
        {
            var pipe = Context.GetPipeline<DeferredRenderPipeline>();
            if (pipe.Pass == DeferredPass.Pass1)
            {
                vao.Use();

                txt0.Use(TextureUnit.Texture0);
                txt1.Use(TextureUnit.Texture1);

                _DefGeometryShader.Use();

                _DefGeometryShader.SetMatrix4("model", GetModelMatrix());
                _DefGeometryShader.SetMatrix4("view", Camera.ViewMatrix);
                _DefGeometryShader.SetMatrix4("projection", Camera.ProjectionMatrix);

                vao.Draw();
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
            vao.Use();

            _ShadowShader.Use();

            foreach (var light in Lights)
            {
                var shadowCamera = light.LightCamera;

                var lightProjection = shadowCamera.ProjectionMatrix;
                var lightView = shadowCamera.ViewMatrix;
                lightSpaceMatrix = lightView * lightProjection;

                _ShadowShader.SetMatrix4("model", GetModelMatrix());
                _ShadowShader.SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);
                _ShadowShader.SetInt("shadowLayer", light.ShadowTextureIndex);

                //GL.CullFace(CullFaceMode.Front);
                vao.Draw();
                //GL.CullFace(CullFaceMode.Back);}
            }
        }

        private List<Matrix4> CubeShadowsMatrices = new List<Matrix4>();

        public void OnRenderCubeShadow()
        {
            vao.Use();

            _CubeShadowShader.Use();

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

                _CubeShadowShader.SetMatrix4("model", GetModelMatrix());
                for (var i = 0; i < CubeShadowsMatrices.Count; i++)
                    _CubeShadowShader.SetMatrix4($"shadowMatrices[{i}]", CubeShadowsMatrices[i]);
                _CubeShadowShader.SetVector3("light.position", light.Position);
                _CubeShadowShader.SetFloat("far_plane", shadowCamera.FarPlane);
                _CubeShadowShader.SetInt("shadowLayer", light.ShadowTextureIndex);

                vao.Draw();
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
            vao.Free();
            _Shader.Free();
            _ShadowShader.Free();
        }

        public void OnReload()
        {
            _Shader.Reload();
            _ShadowShader.Reload();
        }

        public List<ILightObject> Lights => Context.LightObjects;

    }

}
