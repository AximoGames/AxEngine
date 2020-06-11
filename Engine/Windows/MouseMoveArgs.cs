// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo.Engine.Windows
{
    public class MouseMoveArgs
    {
        public Vector2 OldPixelPosition { get; private set; }
        public Vector2 PixelPosition { get; private set; }
        public Vector2 Delta { get; private set; }
        public float DeltaX { get; private set; }
        public float DeltaY { get; private set; }

        public Vector2 OldPosition { get; private set; }
        public Vector2 Position { get; private set; }

        public bool Handled { get; set; }

        internal MouseMoveArgs(MouseMoveEventArgs e)
        {
            PixelPosition = e.Position;
            Delta = e.Delta;
            DeltaX = e.DeltaX;
            DeltaY = e.DeltaY;
            OldPixelPosition = PixelPosition - Delta;

            Position = PixelPosition * SceneContext.Current.ScreenScale;
            OldPosition = OldPixelPosition * SceneContext.Current.ScreenScale;
        }
    }
}
