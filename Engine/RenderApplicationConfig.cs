// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Input;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using OpenToolkit.Windowing.Common.Input;
using OpenToolkit.Windowing.Desktop;

namespace Aximo.Engine
{
    public class RenderApplicationConfig
    {
        public Vector2i WindowSize { get; set; } = new Vector2i(600, 800);
        public string WindowTitle { get; set; } = "AxEngine";
        public WindowBorder WindowBorder { get; set; } = WindowBorder.Fixed;
        public int UpdateFrequency { get; set; } = 60;
        public int RenderFrequency { get; set; } = 60;
        public int IdleUpdateFrequency { get; set; } = 30;
        public int IdleRenderFrequency { get; set; } = 30;
        public bool IsMultiThreaded { get; set; } = true;
        public VSyncMode VSync { get; set; } = VSyncMode.Adaptive;
        public bool HideTitleBar { get; set; } = false;
        public bool UseConsole { get; set; } = false;
        public bool UseGtkUI { get; set; } = false;

        public RenderApplicationConfig()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                IsMultiThreaded = false; // MakeCurrent() bug
            }
            else
            {
                IsMultiThreaded = true;
            }
        }
    }
}
