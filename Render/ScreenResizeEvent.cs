// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo
{
    public class ScreenResizeEventArgs
    {
        public Vector2i OldSize { get; private set; }
        public Vector2i Size { get; private set; }

        internal ScreenResizeEventArgs(Vector2i oldSize, Vector2i size)
        {
            OldSize = oldSize;
            Size = size;
        }

    }

    public class MouseMoveArgs
    {
        public Vector2 OldPosition { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Delta { get; private set; }
        public float DeltaX { get; private set; }
        public float DeltaY { get; private set; }

        public bool Handled { get; set; }

        internal MouseMoveArgs(MouseMoveEventArgs e)
        {
            Position = e.Position;
            Delta = e.Delta;
            DeltaX = e.DeltaX;
            DeltaY = e.DeltaY;
            OldPosition = Position - Delta;
        }

    }

    public enum MouseButton
    {
        None = 0,
        Primary = 1,
        Secondary = 2,
        Middle = 4,
        Wheel = 8,
    }

    public class MouseButtonArgs
    {
        public Vector2i OldPosition { get; private set; }
        public Vector2i Position { get; private set; }

        public bool Handled { get; set; }

        public float WheelDelta { get; set; }

        internal MouseButtonArgs(Vector2i oldPosition, Vector2i position)
        {
            OldPosition = oldPosition;
            Position = position;
        }

    }

}
