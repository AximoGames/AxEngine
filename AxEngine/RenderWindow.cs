// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Graphics;
using OpenToolkit.Input;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Desktop;

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

        private RenderApplicationStartup _startup;

        public RenderWindow(RenderApplicationStartup startup)
        : base(new GameWindowSettings { IsMultiThreaded = startup.IsMultiThreaded, UpdateFrequency = startup.UpdateFrequency, RenderFrequency = startup.RenderFrequency }, new NativeWindowSettings { Size = startup.WindowSize })
        {
            _startup = startup;
            VSync = _startup.VSync;

            if (_startup.HideTitleBar && Environment.OSVersion.Platform == PlatformID.Win32NT)
                Win32Native.HideTitleBar();

            Size = _startup.WindowSize;
            var diff = Size - _startup.WindowSize;
            if (diff != Vector2i.Zero)
                Size = _startup.WindowSize - diff;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            if (IsFocused)
            {
                RenderFrequency = _startup.RenderFrequency;
                UpdateFrequency = _startup.UpdateFrequency;
            }
            else
            {
                RenderFrequency = _startup.IdleRenderFrequency;
                UpdateFrequency = _startup.IdleUpdateFrequency;
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

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        // public void Quit()
        // {
        //     base.Exit();
        // }

    }
}
