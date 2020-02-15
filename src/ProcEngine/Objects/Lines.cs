using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{
    public class Lines : GameObject, IRenderableObject
    {

        public Camera Camera => Context.Camera;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;

        public RenderPosition RenderPosition => RenderPosition.Scene;

        private Shader _Shader;

        private float[] _vertices = DataHelper.Cross;

        private VertexArrayObject vao;
        private BufferObject vbo;

        public override void Init()
        {
            _Shader = new Shader("Shaders/lines.vert", "Shaders/lines.frag");

            vbo = new BufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aPos"), 3);
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aColor"), 4);

            vao = new VertexArrayObject(layout, vbo);
            vao.PrimitiveType = PrimitiveType.Lines;
            vao.Create();

            vao.SetData(_vertices);
        }

        public void OnRender()
        {
            vao.Use();

            _Shader.Use();

            _Shader.SetMatrix4("model", ModelMatrix);
            _Shader.SetMatrix4("view", Camera.GetViewMatrix());
            _Shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            _Shader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            _Shader.SetVector3("viewPos", Camera.Position);

            //GL.Disable(EnableCap.DepthTest);
            vao.Draw();
            //GL.Enable(EnableCap.DepthTest);
        }

        public override void Free()
        {
            vao.Free();
            vbo.Free();
            _Shader.Free();
        }

    }

}
