// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

using System.Drawing;
using OpenTK;

namespace Aximo.Render
{
    public class GraphicsTexture
    {
        public GraphicsTexture(Vector2i size) : this(size.X, size.Y) {
        }

        public GraphicsTexture(int width, int height) {
            Image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics = Graphics.FromImage(Image);
            Texture = new Texture(Image);
            Graphics.Clear(Color.Transparent);
            UpdateTexture();
        }

        private Bitmap Image;

        public Graphics Graphics { get; private set; }

        public Texture Texture { get; private set; }

        public void UpdateTexture() {
            // Graphics.Save();
            Graphics.Flush();
            // Graphics.Dispose();
            Texture.SetData(Image);
        }

    }
}
