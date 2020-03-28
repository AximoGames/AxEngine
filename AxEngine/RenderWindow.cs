// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace Aximo.Engine
{

    public class RenderWindow : GameWindow
    {

        //public static Matrix4 CameraMatrix;
        //private float[] MouseSpeed = new float[3];
        //private Vector2 MouseDelta;
        //private float UpDownDelta;
        //private Vector3 CameraLocation;
        //private Vector3 Up = Vector3.UnitZ;
        //private float Pitch = -0.3f;
        //private float Facing = (float)Math.PI / 2 + 0.15f;

        public RenderWindow(int width, int height, string title, bool isSingleThead = true) : base(width, height, GraphicsMode.Default, title, GameWindowFlags.Default, DisplayDevice.Default, 1, 0, GraphicsContextFlags.Default, GraphicsContext.CurrentContext, isSingleThead) { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void OnFocusedChanged(EventArgs e)
        {
            if (Focused)
            {
                TargetRenderFrequency = 1 / 60.0;
                TargetUpdatePeriod = 1 / 60.0;
            }
            else
            {
                TargetRenderFrequency = 1 / 30.0;
                TargetUpdatePeriod = 1 / 30.0;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
        }

        // public void Quit()
        // {
        //     base.Exit();
        // }

    }
}
