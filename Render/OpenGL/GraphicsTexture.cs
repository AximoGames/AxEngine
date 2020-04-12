// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit;
using OpenToolkit.Mathematics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Aximo.Render
{
    public class GraphicsTexture : IDisposable
    {
        public GraphicsTexture(Vector2i size) : this(size.X, size.Y)
        {
        }

        public GraphicsTexture(int width, int height)
        {
            Image = new Image<Rgba32>(width, height);
            Texture = new Texture(Image, "GraphicsTexture");
            UpdateTexture();
        }

        private Image<Rgba32> Image;

        public Texture Texture { get; private set; }

        public void UpdateTexture()
        {
            // Graphics.Save();
            // Graphics.Dispose();
            Texture.SetData(Image);
        }

        public void Dispose()
        {
            Image?.Dispose();
        }
    }
}
