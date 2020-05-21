// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Aximo.Engine.Components.UI
{
    public class UIRect
    {
        public float Top;
        public float Left;
        public float? Width;
        public float? Height;
    }

    [Flags]
    public enum UIDock
    {
        None = 0,
        Top = 1,
        Left = 2,
        Right = 4,
        Bottom = 8,
        Fill = Top | Left | Right | Bottom,
    }
}
