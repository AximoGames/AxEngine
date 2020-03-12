// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Aximo.Render
{
    public class ScreenTextureObject : GameObject, IRenderTarget, IScaleRotate, IPosition
    {
        private Shader _shader;

        public Vector3 Scale { get; set; } = Vector3.One;
        public Vector3 Rotate { get; set; }
        public Vector3 Position { get; set; }

        private float[] _vertices = DataHelper.Quad;

        private VertexArrayObject vao;

        public Texture SourceTexture;

        public string TexturePath;

        public override void Init()
        {
            UsePipeline<ScreenPipeline>();

            _shader = new Shader("Shaders/screen.vert", "Shaders/screen.frag");

            if (!string.IsNullOrEmpty(TexturePath))
            {
                SourceTexture = new Texture(TexturePath);
            }

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_shader.GetAttribLocation("aPos"), 2);
            layout.AddAttribute<float>(_shader.GetAttribLocation("aTexCoords"), 2);

            vao = new VertexArrayObject(layout);
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

        public RectangleF RectangleUV
        {
            set
            {
                var pos = new Vector3(
                    ((value.X + (value.Width / 2f)) * 2) - 1.0f,
                    ((1 - (value.Y + (value.Height / 2f))) * 2) - 1.0f,
                    0);

                var scale = new Vector3(value.Width, -value.Height, 1.0f);
                Position = pos;
                Scale = scale;
            }
        }

        public RectangleF RectanglePixels
        {
            set
            {
                var pos1 = new Vector2(value.X, value.Y) * RenderContext.Current.PixelToUVFactor;
                var pos2 = new Vector2(value.Right, value.Bottom) * RenderContext.Current.PixelToUVFactor;

                RectangleUV = new RectangleF(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y);
            }
        }

        public void OnRender()
        {
            if (!(Renderer.CurrentPipeline is ScreenPipeline))
                return;

            vao.Bind();

            _shader.Bind();
            SourceTexture.Bind();
            _shader.SetMatrix4("model", GetModelMatrix());

            GL.Disable(EnableCap.CullFace);
            vao.Draw();
            GL.Enable(EnableCap.CullFace);
        }

        public override void Free()
        {
            vao.Free();
            _shader.Free();
        }

    }

}
