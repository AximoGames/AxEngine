using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;

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

        public string TexturePath;

        public override void Init()
        {
            UsePipeline<ScreenPipeline>();

            _shader = new Shader("Shaders/screen.vert", "Shaders/screen.frag");

            if (!string.IsNullOrEmpty(TexturePath))
            {
                SourceTexture = new Texture(TexturePath);
            }

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

        public RectangleF RectangleNDC
        {
            set
            {
                var pos = new Vector3(
                    (value.X + value.Width / 2f) * 2 - 1.0f,
                    (1 - (value.Y + value.Height / 2f)) * 2 - 1.0f, 0);

                var scale = new Vector3(value.Width, -value.Height, 1.0f);
                Position = pos;
                Scale = scale;
            }
        }

        public RectangleF RectanglePixels
        {
            set
            {
                var pos1 = (new Vector2(value.X, value.Y) * RenderContext.Current.PixelToUVFactor);
                var pos2 = new Vector2(value.Right, value.Bottom) * RenderContext.Current.PixelToUVFactor;

                RectangleNDC = new RectangleF(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y);
            }
        }

        // private Vector2 GetPos(float x, float y)
        // {
        //     // float scaleX = 1 / (float)RenderContext.Current.ScreenSize.X;
        //     // float scaleY = 1 / (float)RenderContext.Current.ScreenSize.Y;
        //     // return new Vector2(x * scaleX, y * scaleY);
        //     return 
        // }

        public void OnRender()
        {
            if (!(Context.CurrentPipeline is ScreenPipeline))
                return;

            vao.Use();

            _shader.Use();
            SourceTexture.Use();
            _shader.SetMatrix4("model", GetModelMatrix());

            GL.Disable(EnableCap.CullFace);
            vao.Draw();
            GL.Enable(EnableCap.CullFace);
        }

        public override void Free()
        {
            vao.Free();
            vbo.Free();
            _shader.Free();
        }

    }

}
