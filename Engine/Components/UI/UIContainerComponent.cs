﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Engine.Components.UI
{
    public abstract class UIContainerComponent : UIComponent
    {
        public UIAnchors Padding;

        internal override UIAnchors PaddingInternal => Padding;

        internal override void SetChildBounds()
        {
            foreach (var child in UIComponents)
            {
                if (child.Size == Vector2.Zero)
                {
                    child.AbsoluteOuterRect = AbsolutePaddingRect;
                }
                else
                {
                    var location = child.Location;
                    var size = child.Size + child.PaddingInternal.Size + child.Border.Size + child.Margin.Size;

                    //location.X += Border.Left;
                    //location.Y += Border.Top;

                    //size.X -= child.Border.Size.X;
                    //size.Y -= child.Border.Size.Y;

                    child.AbsoluteOuterRect = BoxHelper.FromSize(AbsolutePaddingRect.Min + location, size);
                }
            }
        }
    }
}
