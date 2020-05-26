// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Aximo.Engine.Components.Geometry
{
    public class GraphicsScreenTextureComponent : ScreenTextureComponent
    {
        public GraphicsScreenTextureComponent(Vector2i size)
        {
            Image = new Image<Rgba32>(size.X, size.Y);
            Texture = Texture.GetFromBitmap(Image, null);
            Material.DiffuseTexture = Texture;
            UpdateTexture();
        }

        protected void ResizeImage(Vector2i size)
        {
            if (size.X == 0 || size.Y == 0)
                return;

            Image = new Image<Rgba32>(size.X, size.Y);
            UpdateTexture();
        }

        protected Image<Rgba32> Image { get; private set; }

        public Texture Texture { get; private set; }

        public void UpdateTexture()
        {
            Texture.SetData(Image);
            Update();
        }
    }
}
