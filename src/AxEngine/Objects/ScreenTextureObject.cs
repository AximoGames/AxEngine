using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace AxEngine
{
    public class ScreenTextureObject : GameObject, IRenderTarget, IScaleRotate, IPosition
    {
        private Shader _shader;

        public Vector3 Scale { get; set; } = Vector3.One;
        public Vector3 Rotate { get; set; }
        public Vector3 Position { get; set; }

        private float[] _vertices = DataHelper.Quad;

        private VertexArrayObject vao;
        private BufferObject vbo;

        protected Texture SourceTexture;

        public override void Init()
        {
            UsePipeline<ScreenPipeline>();

            _shader = new Shader("Shaders/screen.vert", "Shaders/screen.frag");

            vbo = new BufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_shader.GetAttribLocation("aPos"), 2);
            layout.AddAttribute<float>(_shader.GetAttribLocation("aTexCoords"), 2);

            vao = new VertexArrayObject(layout, vbo);
            vao.Create();

            vao.SetData(_vertices);
        }

        public Matrix4 GetModelMatrix()
        {
            return Matrix4.CreateScale(Scale)
            * Matrix4.CreateRotationX(Rotate.X)
            * Matrix4.CreateRotationY(Rotate.Y)
            * Matrix4.CreateRotationZ(Rotate.Z)
            * Matrix4.CreateTranslation(Position);
        }

        public void OnRender()
        {
            if (!(Context.CurrentPipeline is ScreenPipeline))
                return;

            vao.Use();

            _shader.Use();
            SourceTexture.Use();
            _shader.SetMatrix4("model", GetModelMatrix());

            //GL.Disable(EnableCap.CullFace);
            vao.Draw();
            //GL.Enable(EnableCap.CullFace);
        }

        public override void Free()
        {
            vao.Free();
            vbo.Free();
            _shader.Free();
        }

    }

}
