// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Aximo.Engine;
using OpenToolkit;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

using Gtk;

namespace Aximo.AxDemo
{
    internal class Program
    {

        private static Thread th;

        private static GtkUI ui;

        public static void Main(string[] args)
        {
            var bits = IntPtr.Size == 4 ? 32 : 64;
            Console.WriteLine($"{bits} Bit System detected. (Pointer Size: {IntPtr.Size} Bytes)");
            Console.WriteLine($"OS: {Environment.OSVersion}");

            ui = new GtkUI();
            ui.Start();

            UIThreadMain();
            return;

            th = new Thread(UIThreadMain);
            th.Start();

            ConsoleLoop();

            demo.Close();
            demo.Dispose();
            th.Abort();
            Environment.Exit(0);
        }

        private static void ConsoleLoop()
        {
            while (true)
            {
                var cmd = Console.ReadLine();
                var args = cmd.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length == 0)
                    continue;
                switch (cmd)
                {
                    case "q":
                        return;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }
        }

        private static RenderApplication demo;

        private static void UIThreadMain()
        {
            demo = new RenderApplicationDemo(new RenderApplicationStartup
            {
                WindowTitle = "AxEngineDemo",
                WindowSize = new Vector2i(800, 600),
                WindowBorder = WindowBorder.Fixed,
                //RenderFrequency = 490,
                //UpdateFrequency = 490,
                //VSync = VSyncMode.Off,
            }); ; ;
            demo.Run();
            Environment.Exit(0);
        }

    }

}
