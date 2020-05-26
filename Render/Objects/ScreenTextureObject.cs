// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render.Objects;
using Aximo.Render.OpenGL;
using Aximo.Render.Pipelines;
using Aximo.VertexData;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;

#pragma warning disable CA1044 // Properties should not be write only

namespace Aximo.Render
{
    public class ScreenTextureObject : RenderObject, IRenderTarget, IScaleRotate, IPosition
    {
        private RendererShader _shader;

        public Vector3 Scale { get; set; } = Vector3.One;
        public Quaternion Rotate { get; set; }
        public Vector3 Position { get; set; }

        private VertexDataPos2UV[] _vertices = DataHelper.NDCQuadInvertedUV;

        private VertexArrayObject vao;

        public RendererTexture SourceTexture { get; set; }

        public override void Init()
        {
            UsePipeline<ScreenPipeline>();

            _shader = new RendererShader("Shaders/screen.vert", "Shaders/screen.frag");

            var layout = VertexLayoutDefinition.CreateDefinitionFromVertexStruct<VertexDataPos2UV>();
            vao = new VertexArrayObject(layout.BindToShader(_shader));
            vao.SetData(BufferData.Create(_vertices));
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
            if (!(Context.CurrentPipeline is ScreenPipeline))
                return;

            vao.Bind();

            _shader.Bind();
            _shader.SetMatrix4("Model", GetModelMatrix());
            SourceTexture.Bind(0);
            _shader.SetInt("screenTexture", 0);

            GraphicsDevice.Default.CullFace = false;
            vao.Draw();
            GraphicsDevice.Default.CullFace = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                Free();
                SourceTexture = null;
            }

            base.Dispose(disposing);
        }

        public override void Free()
        {
            vao.Free();
            _shader.Free();
            SourceTexture = null;
        }
    }
}
