// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Engine;
using Gtk;
using OpenToolkit;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo.AxDemo
{

    internal class Program
    {

        public static void Main(string[] args)
        {
            var config = new RenderApplicationConfig
            {
                WindowTitle = "AxEngineDemo",
                WindowSize = new Vector2i(800, 600),
                WindowBorder = WindowBorder.Fixed,
                //RenderFrequency = 490,
                //UpdateFrequency = 490,
                //VSync = VSyncMode.Off,
                UseGtkUI = true,
                UseConsole = true,
            };

            new GameStartup<RenderApplicationDemo, GtkUI>(config).Start();
        }

    }

}
