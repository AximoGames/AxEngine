// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;

namespace Aximo.Engine.Windows
{
    public class MouseButtonArgs
    {
        public Vector2 OldPixelPosition { get; private set; }
        public Vector2 PixelPosition { get; private set; }

        public Vector2 OldPosition { get; private set; }
        public Vector2 Position { get; private set; }

        public Vector2 OldPositionNDC { get; private set; }
        public Vector2 PositionNDC { get; private set; }

        public MouseButton Button { get; private set; }
        public InputAction Action { get; private set; }
        public KeyModifiers Modifiers { get; private set; }
        public bool IsPressed { get; private set; }

        public bool Handled { get; set; }

        //public float WheelDelta { get; set; }

        internal MouseButtonArgs(Vector2 oldPosition, Vector2 position, MouseButtonEventArgs e)
        {
            OldPositionNDC = oldPosition;
            PositionNDC = position;

            OldPixelPosition = AxMath.MapFromNDC(OldPositionNDC, Application.Current.ScreenPixelSize);
            PixelPosition = AxMath.MapFromNDC(PositionNDC, Application.Current.ScreenPixelSize);

            Button = e.Button;
            Action = e.Action;
            Modifiers = e.Modifiers;
            IsPressed = e.IsPressed;

            Position = PixelPosition * SceneContext.Current.ScreenScale;
            OldPosition = OldPixelPosition * SceneContext.Current.ScreenScale;
        }
    }
}
