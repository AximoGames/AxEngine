using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{
    public class Grid : GameObject, IRenderableObject
    {

        public Camera Camera => Context.Camera;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;

        public RenderPosition RenderPosition => RenderPosition.Scene;

        private Shader _Shader;

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

            var _vertices = new List<float>();

            var size = 10;
            var color = new float[] { 0.45f, 0.45f, 0.0f, 1.0f };

            for (var i = -size; i <= size; i++)
            {
                _vertices.AddRange(new float[] { -size, i, 0 });
                _vertices.AddRange(color);

                _vertices.AddRange(new float[] { size, i, 0 });
                _vertices.AddRange(color);

                _vertices.AddRange(new float[] { i, -size, 0 });
                _vertices.AddRange(color);

                _vertices.AddRange(new float[] { i, size, 0 });
                _vertices.AddRange(color);
            }

            vao.SetData(_vertices.ToArray());
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
