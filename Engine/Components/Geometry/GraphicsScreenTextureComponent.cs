// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Aximo.Engine
{
    public class GraphicsScreenTextureComponent : ScreenTextureComponent
    {
        public GraphicsScreenTextureComponent(Vector2i size)
        {
            Image = new Image<Rgba32>(size.X, size.Y);
            Texture = GameTexture.GetFromBitmap(Image, null);
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

        public GameTexture Texture { get; private set; }

        public void UpdateTexture()
        {
            Texture.SetData(Image);
            Update();
        }
    }
}
