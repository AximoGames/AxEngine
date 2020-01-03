using LearnOpenTK.Common;
using OpenTK;
using System.Collections.Generic;

namespace ProcEngine
{

    public class LightObject : GameObject, IRenderableObject, IPosition, ILightObject
    {
        public Cam Camera => Context.Camera;

        public Vector3 Position { get; set; }

        public RenderPosition RenderPosition => RenderPosition.Scene;

        private Shader _shader;
        private VertexArrayObject vao;
        private VertexBufferObject vbo;

        private float[] _vertices = DataHelper.Cube;

        public override void Init()
        {
            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            vbo = new VertexBufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_shader.GetAttribLocation("aPos"), 3);
            layout.AddAttribute<float>(_shader.GetAttribLocation("aNormal"), 3);
            layout.AddAttribute<float>(_shader.GetAttribLocation("aTexCoords"), 2);

            vao = new VertexArrayObject(layout, vbo);
            vao.Create();

            vao.SetData(_vertices);
        }

        public void OnRender()
        {
            vao.Use();
            _shader.Use();

            Matrix4 lampMatrix = Matrix4.Identity;
            lampMatrix *= Matrix4.CreateScale(0.2f);
            lampMatrix *= Matrix4.CreateTranslation(Position);

            _shader.SetMatrix4("model", lampMatrix);
            _shader.SetMatrix4("view", Camera.GetViewMatrix());
            _shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            vao.Draw();
        }

        public override void Free()
        {
            vao.Free();
            vbo.Free();
            _shader.Free();
        }

    }

}
