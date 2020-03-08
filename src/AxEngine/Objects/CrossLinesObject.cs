using OpenToolkit.Mathematics;
using OpenToolkit.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace AxEngine
{
    public class CrossLinesObject : GameObject, IRenderableObject
    {

        public Camera Camera => Context.Camera;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;

        private Shader _Shader;

        private float[] _vertices = DataHelper.Cross;

        private VertexArrayObject vao;

        public override void Init()
        {
            UsePipeline<ForwardRenderPipeline>();

            _Shader = new Shader("Shaders/lines.vert", "Shaders/lines.frag");

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aPos"), 3);
            layout.AddAttribute<float>(_Shader.GetAttribLocation("aColor"), 4);

            vao = new VertexArrayObject(layout);
            vao.PrimitiveType = PrimitiveType.Lines;
            vao.SetData(_vertices);
        }

        public void OnRender()
        {
            vao.Bind();

            _Shader.Bind();

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
            _Shader.Free();
        }

    }

}
