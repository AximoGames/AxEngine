using LearnOpenTK;
using LearnOpenTK.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{
    public class TestObject : GameObject, IRenderableObject, IShadowObject, IReloadable
    {

        public Cam Camera => Context.Camera;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;

        public bool Debug;

        public ILightObject Light;

        private Shader _Shader;
        private Shader _ShadowShader;

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

            _ShadowShader = new Shader("Shaders/shadow.vert", "Shaders/shadow.frag");

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

            _Shader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            _Shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _Shader.SetVector3("lightPos", Light.Position);
            _Shader.SetVector3("viewPos", Camera.Position);

            vao.Draw();
        }

        private Matrix4 lightSpaceMatrix;

        public void OnRenderShadow()
        {
            vao.Use();

            _ShadowShader.Use();

            float near_plane = 0.01f;
            float far_plane = 7.5f;


            //            var lightProjection = Matrix4.CreateOrthographic(20, 20, near_plane, far_plane);
            var lightProjection = Matrix4.CreateOrthographicOffCenter(-12, 12, -12, 12, near_plane, far_plane);
            //var lightProjection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 2.0f, 1.0f, 0.1f, 100f);
            var lightView = Matrix4.LookAt(Light.Position, new Vector3(0, 0, 0), new Vector3(0, 0, 1));
            lightSpaceMatrix =  lightView * lightProjection ;

            _ShadowShader.SetMatrix4("model", ModelMatrix);
            _ShadowShader.SetMatrix4("view", lightView);
            _ShadowShader.SetMatrix4("projection", lightProjection);

            //_ShadowShader.SetMatrix4("model", ModelMatrix);
            //_ShadowShader.SetMatrix4("view", Camera.GetViewMatrix());
            //_ShadowShader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            vao.Draw();
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
    }

}
