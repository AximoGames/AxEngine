// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public struct UIAnchors
    {

        public UIAnchors(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static UIAnchors Zero => new UIAnchors();

        public Vector2 Min => new Vector2(Left, Top);
        public Vector2 Max => new Vector2(Right, Bottom);

        public float Top;
        public float Left;
        public float Right;
        public float Bottom;

        public float Width => Left + Right;
        public float Height => Top + Bottom;

        public Vector2 Size => new Vector2(Width, Height);
    }

}
