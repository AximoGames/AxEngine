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
        private static Serilog.ILogger Log = Aximo.Log.ForContext<RenderWindow>();

        private RenderApplicationConfig Config;

        public RenderWindow(RenderApplicationConfig config)
        : base(new GameWindowSettings { IsMultiThreaded = config.IsMultiThreaded, UpdateFrequency = config.UpdateFrequency, RenderFrequency = config.RenderFrequency }, new NativeWindowSettings { Size = config.WindowSize })
        {
            Log.Verbose("Created window");
            Config = config;
            Title = Config.WindowTitle;
            VSync = Config.VSync;

            if (Config.HideTitleBar && Environment.OSVersion.Platform == PlatformID.Win32NT)
                Win32Native.HideTitleBar();

            Size = Config.WindowSize;
            var diff = Size - Config.WindowSize;
            if (diff != Vector2i.Zero)
                Size = Config.WindowSize - diff;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            if (IsFocused)
            {
                RenderFrequency = Config.RenderFrequency;
                UpdateFrequency = Config.UpdateFrequency;
            }
            else
            {
                RenderFrequency = Config.IdleRenderFrequency;
                UpdateFrequency = Config.IdleUpdateFrequency;
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
