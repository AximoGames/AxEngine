// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;

namespace Aximo.Engine
{
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
}
