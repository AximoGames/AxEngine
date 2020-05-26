// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Engine.Components.UI
{
    public class UIFloatingContainer : UIContainerComponent
    {
        internal override void SetChildBounds()
        {
            foreach (var child in UIComponents)
            {
                var location = child.Location;
                var size = child.Size + Padding.Size + Border.Size + Margin.Size;
                child.AbsoluteOuterRect = BoxHelper.FromSize(AbsolutePaddingRect.Min + location, size);
            }
        }
    }
}
