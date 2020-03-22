// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Render;
using OpenTK;

namespace Aximo.Engine
{

    public class GraphicsScreenTextureComponent : ScreenTextureComponent
    {
        public GraphicsScreenTextureComponent(int width, int height)
        {
            Image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics = Graphics.FromImage(Image);
            Texture = GameTexture.GetFromBitmap(Image, null);
            Material.DiffuseTexture = Texture;
            UpdateTexture();
        }

        private Bitmap Image;

        public Graphics Graphics { get; private set; }

        public GameTexture Texture { get; private set; }

        public void UpdateTexture()
        {
            // Graphics.Save();
            Graphics.Flush();
            // Graphics.Dispose();
            Texture.SetData(Image);
            Update();
        }

    }

}
