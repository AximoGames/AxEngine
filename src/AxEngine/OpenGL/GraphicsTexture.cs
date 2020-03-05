using System.Drawing;
using OpenTK;

namespace AxEngine
{
    public class GraphicsTexture
    {
        public GraphicsTexture(Vector2i size)
        {
            Image = new Bitmap(size.X, size.Y);
            Graphics = Graphics.FromImage(Image);
            Texture = new Texture(Image);
        }

        private Bitmap Image;

        public Graphics Graphics { get; private set; }

        public Texture Texture { get; private set; }

        public void UpdateTexture()
        {
            Graphics.Flush();
            Texture.SetData(Image);
        }

    }
}
