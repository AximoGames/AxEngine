// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public abstract class UIContainerComponent : UIComponent
    {
        public UIAnchors Padding;

        protected internal override UIAnchors PaddingInternal => Padding;

        internal override void SetChildBounds()
        {
            foreach (var child in UIComponents)
            {
                child.AbsoluteOuterRect = AbsolutePaddingRect;
            }
        }
    }
}
