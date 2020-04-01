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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Aximo.Engine
{

    public class GraphicsScreenTextureComponent : ScreenTextureComponent
    {
        public GraphicsScreenTextureComponent(int width, int height)
        {
            Image = new Image<Rgba32>(width, height);
            Texture = GameTexture.GetFromBitmap(Image, null);
            Material.DiffuseTexture = Texture;
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
