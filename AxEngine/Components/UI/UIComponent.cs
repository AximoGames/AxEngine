// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SixLabors.Primitives;

namespace Aximo.Engine
{
    public class UIComponent : GraphicsScreenTextureComponent
    {

        public UIComponent() : base(100, 100)
        {
        }

        public UIAnchors Margin;
        public UIRect ClientRect;

        protected RectangleF GlobalRect;
    }

}
