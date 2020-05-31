// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Render.OpenGL;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo.Engine
{
    /// <summary>
    /// Initial configuration of <see cref="Application"/>
    /// </summary>
    public class ApplicationConfig
    {
        public Vector2i WindowSize { get; set; } = new Vector2i(800, 600);
        public Vector2i? WindowLocation { get; set; }
        public string WindowTitle { get; set; } = "AxEngine";
        public WindowBorder WindowBorder { get; set; } = WindowBorder.Fixed;
        public int UpdateFrequency { get; set; } = 60;
        public int RenderFrequency { get; set; } = 60;
        public int IdleUpdateFrequency { get; set; } = 30;
        public int IdleRenderFrequency { get; set; } = 30;
        public bool IsMultiThreaded { get; set; } = true;
        public VSyncMode VSync { get; set; } = VSyncMode.Adaptive;
        public bool HideTitleBar { get; set; } = false;
        public bool UseConsole { get; set; } = true;
        public bool UseGtkUI { get; set; } = false;
        public bool UseShadows { get; set; } = true;
        public bool UseFrameDebug { get; set; } = true;

        /// <summary>
        /// Get or set when rendering backend should be flushed.
        /// Usefull when measuring draw call times.
        /// </summary>
        public FlushRenderBackend FlushRenderBackend { get; set; } = FlushRenderBackend.None;

        public ApplicationConfig()
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
