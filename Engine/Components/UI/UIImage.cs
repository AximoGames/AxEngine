// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

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
}
