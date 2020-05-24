// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Engine.Components.UI
{
    public class UIImage : UIComponent
    {
        public UIImage(Material material) : this(material, new Vector2i(100, 100))
        {
        }

        public UIImage(Material material, Vector2i size) : base(size)
        {
            Material = material;
        }

        public UIImage(string imagePath) : this(Texture.GetFromFile(imagePath), new Vector2i(100, 100))
        {
        }

        public UIImage(string imagePath, Vector2i size) : this(Texture.GetFromFile(imagePath), size)
        {
        }

        public UIImage(Texture image) : this(image, new Vector2i(100, 100))
        {
        }

        public UIImage(Texture image, Vector2i size) : base(size)
        {
            Material.DiffuseTexture = image;
        }
    }
}
