using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace ProcEngine
{

    public class Skybox : GameObject, IRenderableObject
    {
        public Camera Camera => Context.Camera;

        public RenderPosition RenderPosition => RenderPosition.Scene;

        private Shader _shader;
        private VertexArrayObject vao;
        private VertexBufferObject vbo;

        private Texture txt;

        private float[] _vertices = DataHelper.SkyBox;

        public override void Init()
        {
            _shader = new Shader("Shaders/skybox.vert", "Shaders/skybox.frag");
            //txt = Texture.LoadCubeMap("Ressources/desert-skybox/#.tga");
            txt = Texture.LoadCubeMap("Ressources/water-skybox/#.jpg");

            vbo = new VertexBufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_shader.GetAttribLocation("aPos"), 3);

            vao = new VertexArrayObject(layout, vbo);
            vao.Create();

            vao.SetData(_vertices);
        }

        public void OnRender()
        {
            vao.Use();
            _shader.Use();

            _shader.SetMatrix4("view", Camera.GetViewMatrix(Vector3.Zero));
            _shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            txt.Use(TextureUnit.Texture0);
            _shader.SetInt("skybox", 0);

            GL.DepthMask(false);
            vao.Draw();
            GL.DepthMask(true);
        }

        public override void Free()
        {
            vao.Free();
            vbo.Free();
            _shader.Free();
        }

    }

}
