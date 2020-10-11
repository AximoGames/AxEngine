// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.Runtime.Loader;
using Aximo.Engine;
using McMaster.NETCore.Plugins;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo.AxDemo
{

    internal class Program
    {

        public static int Counter = 0;

        public static void Main(string[] args)
        {
            //var obj = PluginManager.GetScriptBehaviour<TestClass>();
            //obj.Invoke();

            var config = new ApplicationConfig
            {
                WindowTitle = "AxEngineDemo",
                WindowSize = new Vector2i(800, 600),
                WindowBorder = WindowBorder.Resizable,
                IsMultiThreaded = true,
                RenderFrequency = 0,
                UpdateFrequency = 0,
                IdleRenderFrequency = 0,
                IdleUpdateFrequency = 0,
                VSync = VSyncMode.Off,
                // UseGtkUI = true,
                UseConsole = true,
                NormalizedUISize = new Vector2(800, 600),
                //UseFrameDebug = true,
            };
            new DemoApplication().Start(config);
        }
    }
}
