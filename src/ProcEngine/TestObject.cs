using LearnOpenTK.Common;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{
    public class TestObject : GameObject, IRenderableObject
    {

        public Cam Camera => Context.Camera;
        public Matrix4 ModelMatrix { get; set; } = Matrix4.Identity;

        public bool Debug;

        public ILightObject Light;

        private Shader _shader;

        private float[] _vertices = DataHelper.Cube;

        private VertexArrayObject vao;
        private VertexBufferObject vbo;

        public override void Init()
        {
            if (Debug)
                _vertices = DataHelper.CubeDebug;

            _shader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");

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

            _shader.SetMatrix4("model", ModelMatrix);
            _shader.SetMatrix4("view", Camera.GetViewMatrix());
            _shader.SetMatrix4("projection", Camera.GetProjectionMatrix());

            _shader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            _shader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _shader.SetVector3("lightPos", Light.Position);
            _shader.SetVector3("viewPos", Camera.Position);

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
