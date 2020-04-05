// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;
using OpenToolkit.Mathematics;
using SixLabors.Primitives;

namespace Aximo.Engine
{

    public class UIImage : UIComponent
    {

        public UIImage(GameMaterial material) : this(material, new Vector2i(100, 100))
        {
        }

        public UIImage(GameMaterial material, Vector2i size) : base(size)
        {
            Material = material;
        }

        public UIImage(string imagePath) : this(GameTexture.GetFromFile(imagePath), new Vector2i(100, 100))
        {
        }

        public UIImage(string imagePath, Vector2i size) : this(GameTexture.GetFromFile(imagePath), size)
        {
        }

        public UIImage(GameTexture image) : this(image, new Vector2i(100, 100))
        {
        }

        public UIImage(GameTexture image, Vector2i size) : base(size)
        {
            Material.DiffuseTexture = image;
        }
    }

    public class UIComponent : GraphicsScreenTextureComponent
    {

        public UIComponent() : this(new Vector2i(100, 100))
        {
        }

        public UIComponent(Vector2i size) : base(size)
        {
        }

        public UIAnchors Margin;
        public UIRect ClientRect;

        private RectangleF? _RectangleUV;
        public RectangleF RectangleUV
        {
            set
            {
                if (_RectangleUV == value)
                    return;

                var pos = new Vector3(
                    ((value.X + (value.Width / 2f)) * 2) - 1.0f,
                    ((1 - (value.Y + (value.Height / 2f))) * 2) - 1.0f,
                    0);

                var scale = new Vector3(value.Width, -value.Height, 1.0f);
                RelativeTranslation = pos;
                RelativeScale = scale;
                _RectanglePixels = null;
                _RectangleUV = value;
            }
        }

        private RectangleF? _RectanglePixels;
        public RectangleF RectanglePixels
        {
            set
            {
                if (_RectanglePixels == value)
                    return;

                var pos1 = new Vector2(value.X, value.Y) * RenderContext.Current.PixelToUVFactor;
                var pos2 = new Vector2(value.Right, value.Bottom) * RenderContext.Current.PixelToUVFactor;

                RectangleUV = new RectangleF(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y);
                _RectangleUV = null;
                _RectanglePixels = value;
            }
        }
    }

}
