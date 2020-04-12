// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public class UIFlowContainer : UIContainerComponent
    {
        public Vector2 DefaultChildSizes;
        public UIAnchors ExtraChildMargin;

        internal override void SetChildBounds()
        {
            var location = Vector2.Zero;
            foreach (var child in UIComponents)
            {
                var extra = child.PaddingInternal.Size + child.Border.Size + child.Margin.Size;

                var defaultSize = DefaultChildSizes;
                if (defaultSize.X == 0)
                    defaultSize.X = AbsolutePaddingRect.Size.X - extra.X - ExtraChildMargin.Size.X;
                if (defaultSize.Y == 0)
                    defaultSize.Y = AbsolutePaddingRect.Size.Y - extra.Y - ExtraChildMargin.Size.Y;

                var size = child.Size;
                if (size.X == 0)
                    size.X = defaultSize.X;
                if (size.Y == 0)
                    size.Y = defaultSize.Y;

                size += extra;
                child.AbsoluteOuterRect = BoxHelper.FromSize(AbsolutePaddingRect.Min + location + ExtraChildMargin.Min, size);
                location.Y += size.Y + ExtraChildMargin.Size.Y; // TODO: Use X for other alignment
            }
        }
    }
}
