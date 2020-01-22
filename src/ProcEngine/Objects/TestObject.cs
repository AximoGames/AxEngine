using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{
    public class TestObject : GameObject, IRenderableObject, IShadowObject, IReloadable, ILightTarget
    {

        public Camera Camera => Context.Camera;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;

        public RenderPosition RenderPosition => RenderPosition.Scene;

        public bool Debug;

        private Shader _Shader;
        private Shader _ShadowShader;
        private Shader _CubeShadowShader;

        private float[] _vertices = DataHelper.Cube;

        private VertexArrayObject vao;
        private VertexBufferObject vbo;

        private Texture txt0;
        private Texture txt1;

        public override void Init()
        {
            if (Debug)
                _vertices = DataHelper.CubeDebug;

            _Shader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");

            txt0 = new Texture("Ressources/woodenbox.png");
            txt1 = new Texture("Ressources/woodenbox_specular.png");

            _ShadowShader = new Shader("Shaders/shadow-directional.vert", "Shaders/shadow-directional.frag");
            _CubeShadowShader = new Shader("Shaders/shadow-cube.vert", "Shaders/shadow-cube.frag", "Shaders/shadow-cube.geom");

            vbo = new VertexBufferObject();
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

        public void OnRender()
        {
            vao.Use();

            txt0.Use(TextureUnit.Texture0);
            txt1.Use(TextureUnit.Texture1);
            Window.shadowFb.DestinationTexture.Use(TextureUnit.Texture2);
            var debugMatrix = new Matrix4(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 0, 1));

            _Shader.Use();

            _Shader.SetMatrix4("model", ModelMatrix);
            _Shader.SetMatrix4("view", Camera.GetViewMatrix());
            _Shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            _Shader.SetMatrix4("lightSpaceMatrix", lightSpaceMatrix);

            _Shader.SetInt("material.diffuse", 0);
            _Shader.SetInt("material.specular", 1);
            _Shader.SetInt("shadowMap", 2);
            _Shader.SetMatrix4("debugMatrix", debugMatrix);

            ILightObject light = Lights[0];

            _Shader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            _Shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _Shader.SetVector3("lightPos", light.Position);
            _Shader.SetVector3("viewPos", Camera.Position);

            vao.Draw();
        }

        private Matrix4 lightSpaceMatrix;

        public void OnRenderShadow()
        {
            vao.Use();

            _ShadowShader.Use();

            ILightObject light = Lights[0];

            var shadowCamera = new OrthographicCamera(light.Position)
            {
                NearPlane = 0.01f,
                FarPlane = 7.5f,
            };
            shadowCamera.LookAt = new Vector3(0);

            var lightProjection = shadowCamera.GetProjectionMatrix();
            var lightView = shadowCamera.GetViewMatrix();
            lightSpaceMatrix = lightView * lightProjection;

            _ShadowShader.SetMatrix4("model", ModelMatrix);
            _ShadowShader.SetMatrix4("view", lightView);
            _ShadowShader.SetMatrix4("projection", lightProjection);

            vao.Draw();
        }

        private List<Matrix4> CubeShadowsMatrices = new List<Matrix4>();

        public void OnRenderCubeShadow()
        {
            vao.Use();

            _CubeShadowShader.Use();

            ILightObject light = Lights[1]; // TODO

            var shadowCamera = new PerspectiveFieldOfViewCamera(light.Position, 1.0f)
            {
                NearPlane = 0.1f,
                FarPlane = 25f,
            };

            CubeShadowsMatrices.Clear();

            AddShadowCubeMatrix(shadowCamera, new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f));
            AddShadowCubeMatrix(shadowCamera, new Vector3(-1.0f, 0.0f, 0.0f), new Vector3(0.0f, -1.0f, 0.0f));
            AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f));
            AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, -1.0f, 0.0f), new Vector3(0.0f, 0.0f, -1.0f));
            AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f));
            AddShadowCubeMatrix(shadowCamera, new Vector3(0.0f, 0.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f));

            _CubeShadowShader.SetMatrix4("model", ModelMatrix);
            for (var i = 0; i < CubeShadowsMatrices.Count; i++)
                _CubeShadowShader.SetMatrix4($"shadowMatrices[{i}]", CubeShadowsMatrices[i]);
            _CubeShadowShader.SetVector3("lightPos", light.Position);
            _CubeShadowShader.SetFloat("far_plane", shadowCamera.FarPlane);

            vao.Draw();
        }

        private void AddShadowCubeMatrix(Camera camera, Vector3 direction, Vector3 up)
        {
            var proj = camera.GetProjectionMatrix();
            var view = Matrix4.LookAt(camera.Position, camera.Position + direction, up);
            CubeShadowsMatrices.Add(view * proj);
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
