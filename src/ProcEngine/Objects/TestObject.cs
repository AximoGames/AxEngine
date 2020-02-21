using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
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

    public class TestObject : RenderableObject, IShadowObject, IReloadable, ILightTarget, IScaleRotate,
        IForwardRenderable, IDeferredRenderable
    {

        public Camera Camera => Context.Camera;
        public Vector3 Position { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Rotate { get; set; }

        public bool RenderShadow { get; set; } = true;

        public Matrix4 GetModelMatrix()
        {
            return Matrix4.CreateScale(Scale)
            * Matrix4.CreateRotationX(Rotate.X)
            * Matrix4.CreateRotationY(Rotate.Y)
            * Matrix4.CreateRotationZ(Rotate.Z)
            * Matrix4.CreateTranslation(Position);
        }

        public bool Debug;

        private Shader _Shader;
        private Shader _DefGeometryShader;
        private Shader _ShadowShader;
        private Shader _CubeShadowShader;

        private float[] _vertices = DataHelper.Cube;

        private VertexArrayObject vao;
        private BufferObject vbo;

        private Texture txt0;
        private Texture txt1;

        public IRenderPipeline PrimaryRenderPipeline;

        public override void Init()
        {
            UsePipeline<PointShadowRenderPipeline>();
            UsePipeline<DirectionalShadowRenderPipeline>();

            if (PrimaryRenderPipeline == null)
                PrimaryRenderPipeline = Context.GetPipeline<DeferredRenderPipeline>();

            UsePipeline(PrimaryRenderPipeline);

            if (Debug)
                _vertices = DataHelper.CubeDebug;

            _Shader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _DefGeometryShader = new Shader("Shaders/deferred-gbuffer.vert", "Shaders/deferred-gbuffer.frag");

            txt0 = new Texture("Ressources/woodenbox.png");
            txt1 = new Texture("Ressources/woodenbox_specular.png");

            _ShadowShader = new Shader("Shaders/shadow-directional.vert", "Shaders/shadow-directional.frag");
            _CubeShadowShader = new Shader("Shaders/shadow-cube.vert", "Shaders/shadow-cube.frag", "Shaders/shadow-cube.geom");

            vbo = new BufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aPos"), 3);
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aNormal"), 3);
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aTexCoords"), 2);

            vao = new VertexArrayObject(layout, vbo);
            vao.Create();

            vao.SetData(_vertices);
        }

        public void OnForwardRender()
        {
            vao.Use();

            txt0.Use(TextureUnit.Texture0);
            txt1.Use(TextureUnit.Texture1);
            Context.GetPipeline<DirectionalShadowRenderPipeline>().FrameBuffer.DestinationTexture.Use(TextureUnit.Texture2);

            _Shader.Use();

            _Shader.SetMatrix4("model", GetModelMatrix());
            _Shader.SetMatrix4("view", Camera.GetViewMatrix());
            _Shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            _Shader.SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);

            _Shader.SetInt("shadowMap", 2);

            _Shader.SetVector3("material.color", new Vector3(1.0f, 1.0f, 0f));
            _Shader.SetInt("material.diffuse", 0);
            _Shader.SetInt("material.specular", 1);
            _Shader.SetFloat("material.ambient", 0.3f);
            _Shader.SetFloat("material.shininess", 32f);
            _Shader.SetFloat("material.specularStrength", 0.5f);

            //_Shader.SetVector3("light.position", GetShadowLight().Position);
            //_Shader.SetVector3("light.color", new Vector3(0.5f, 0.5f, 0.5f));
            _Shader.SetVector3("viewPos", Camera.Position);

            var shadowCamera = GetCubeShadowCamera();
            _Shader.SetFloat("far_plane", shadowCamera.FarPlane);
            Context.GetPipeline<PointShadowRenderPipeline>().FrameBuffer.DestinationTexture.Use(TextureUnit.Texture3);
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
                _DefGeometryShader.SetMatrix4("view", Camera.GetViewMatrix());
                _DefGeometryShader.SetMatrix4("projection", Camera.GetProjectionMatrix());

                vao.Draw();
            }

            if (pipe.Pass == DeferredPass.Pass2)
            {
            }
        }

        private Matrix4 lightSpaceMatrix;

        private Camera GetShadowCamera()
        {
            ILightObject light = GetShadowLight();

            //var shadowCamera = new PerspectiveFieldOfViewCamera(light.Position, 1.0f)
            var shadowCamera = new OrthographicCamera(light.Position)
            {
                NearPlane = 1.0f,
                FarPlane = 25f,
            };
            var box = Context.GetObjectByName("Box1");
            if (box != null)
                shadowCamera.LookAt = (box as IPosition).Position;
            else
                shadowCamera.LookAt = new Vector3(0, 0, 0);

            return shadowCamera;
        }

        private Camera GetCubeShadowCamera()
        {
            ILightObject light = GetCubeShadowLight(); // TODO

            var shadowCamera = new PerspectiveFieldOfViewCamera(light.Position, 1.0f)
            {
                NearPlane = 0.1f,
                FarPlane = 25f,
                Fov = 90f,
            };

            return shadowCamera;
        }

        // Because of shared variables in the shaders, only one position is possible yet
        private int TMP_LIGHT_IDX = 0; // 1= moving, 0=static light

        private ILightObject GetCubeShadowLight()
        {
            return Lights[TMP_LIGHT_IDX];
        }

        private ILightObject GetShadowLight()
        {
            return Lights[TMP_LIGHT_IDX];
        }

        public void OnRenderShadow()
        {
            vao.Use();

            _ShadowShader.Use();

            var shadowCamera = GetShadowCamera();

            var lightProjection = shadowCamera.GetProjectionMatrix();
            var lightView = shadowCamera.GetViewMatrix();
            lightSpaceMatrix = lightView * lightProjection;

            _ShadowShader.SetMatrix4("model", GetModelMatrix());
            _ShadowShader.SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);

            //GL.CullFace(CullFaceMode.Front);
            vao.Draw();
            //GL.CullFace(CullFaceMode.Back);
        }

        private List<Matrix4> CubeShadowsMatrices = new List<Matrix4>();

        public void OnRenderCubeShadow()
        {
            vao.Use();

            _CubeShadowShader.Use();

            var shadowCamera = GetCubeShadowCamera();

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
            _CubeShadowShader.SetVector3("light.position", GetCubeShadowLight().Position);
            _CubeShadowShader.SetFloat("far_plane", shadowCamera.FarPlane);
            _CubeShadowShader.SetInt("shadowLayer", 0);

            vao.Draw();
        }

        public const bool CUBE_MAP_SHADOW_ROTATED = true;

        private void AddShadowCubeMatrix(Camera camera, Vector3 direction, Vector3 up)
        {
            var proj = camera.GetProjectionMatrix();
            var view = Matrix4.LookAt(camera.Position, camera.Position + direction, up);
            if (CUBE_MAP_SHADOW_ROTATED)
                CubeShadowsMatrices.Add(view * proj);
            else
                CubeShadowsMatrices.Add(view * proj * Matrix4.CreateScale(1, -1, 1));
        }

        public override void Free()
        {
            vao.Free();
            vbo.Free();
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
