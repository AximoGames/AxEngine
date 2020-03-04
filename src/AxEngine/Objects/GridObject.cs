using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace AxEngine
{
    public class GridObject : GameObject, IRenderableObject
    {

        public Camera Camera => Context.Camera;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;

        public int Size = 10;
        public bool Center = true;

        private Shader _Shader;

        private VertexArrayObject vao;
        private VertexBufferObject vbo;

        public override void Init()
        {
            UsePipeline<ForwardRenderPipeline>();

            _Shader = new Shader("Shaders/lines.vert", "Shaders/lines.frag");

            vbo = new VertexBufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aPos"), 3);
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aColor"), 4);

            vao = new VertexArrayObject(layout, vbo);
            vao.PrimitiveType = PrimitiveType.Lines;
            vao.Create();

            var _vertices = new List<float>();

            var size = Size;
            var color = new float[] { 0.45f, 0.45f, 0.0f, 1.0f };

            int start;
            int end;
            float startPos;
            float endPos;
            if (Center)
            {
                start = -size;
                end = size;
                startPos = -size;
                endPos = size;
            }
            else
            {
                start = 0;
                end = size;
                startPos = 0f;
                endPos = size;
            }

            for (var i = start; i <= end; i++)
            {
                _vertices.AddRange(new float[] { startPos, i, 0 });
                _vertices.AddRange(color);

                _vertices.AddRange(new float[] { endPos, i, 0 });
                _vertices.AddRange(color);

                _vertices.AddRange(new float[] { i, startPos, 0 });
                _vertices.AddRange(color);

                _vertices.AddRange(new float[] { i, endPos, 0 });
                _vertices.AddRange(color);
            }

            vao.SetData(_vertices.ToArray());
        }

        public void OnRender()
        {
            vao.Use();

            _Shader.Use();

            _Shader.SetMatrix4("model", ModelMatrix);
            _Shader.SetMatrix4("view", Camera.ViewMatrix);
            _Shader.SetMatrix4("projection", Camera.ProjectionMatrix);

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
