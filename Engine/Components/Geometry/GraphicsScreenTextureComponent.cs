// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Aximo.Engine.Components.Geometry
{
    public class GraphicsScreenTextureComponent : ScreenTextureComponent
    {
        public GraphicsScreenTextureComponent(Vector2 size)
        {
            var realSize = TransformSize(size);
            Image = new Image<Rgba32>(realSize.X, realSize.Y);
            ImageContext = new ImageContext(Image, SceneContext.Current.ScaleToPixelFactor);
            Texture = Texture.GetFromBitmap(Image, null);
            Material.DiffuseTexture = Texture;
            UpdateTexture();
        }

        private Vector2i TransformSize(Vector2 size) => (size * SceneContext.Current.ScaleToPixelFactor).Round().ToVector2i();

        protected void ResizeImage(Vector2 size)
        {
            var realSize = TransformSize(size);

            if (realSize.X == 0 || realSize.Y == 0)
                return;

            Image = new Image<Rgba32>(realSize.X, realSize.Y);
            ImageContext.SetImage(Image);
            UpdateTexture();
        }

        protected Image<Rgba32> Image { get; private set; }
        protected ImageContext ImageContext { get; private set; }

        public Texture Texture { get; private set; }

        public void UpdateTexture()
        {
            Texture.SetData(Image);
            PropertyChanged();
        }
    }
}
