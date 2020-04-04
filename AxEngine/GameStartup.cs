// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Aximo.Engine;

namespace Aximo.Engine
{
    public class GameStartup<TApp, TGtk>
        where TApp : RenderApplication
        where TGtk : GtkUI
    {

        private RenderApplicationConfig Config;

        public GameStartup(RenderApplicationConfig config)
        {
            Config = config;
        }

        private static Serilog.ILogger Log = Aximo.Log.ForContext<GameStartup<TApp, TGtk>>();

        private Thread th;

        private GtkUI ui;

        public void Start()
        {
            SharedLib.EnableLogging();
            var bits = IntPtr.Size == 4 ? 32 : 64;
            Log.Verbose($"{bits} Bit System detected. (Pointer Size: {IntPtr.Size} Bytes)");
            Log.Verbose("OS: {OSVersion}", Environment.OSVersion);

            if (Config.UseGtkUI)
            {
                ui = new GtkUI();
                ui.Start();
            }

            if (Config.UseConsole)
            {
                th = new Thread(UIThreadMain);
                th.Start();

                ConsoleLoop();

                demo.Close();
                demo.Dispose();
                th.Abort();
                Environment.Exit(0);
            }
            else
            {
                UIThreadMain();
                Environment.Exit(0);
            }
        }

        private RenderApplication demo;

        private void UIThreadMain()
        {
            demo = (TApp)Activator.CreateInstance(typeof(TApp), Config);
            demo.Run();
            Environment.Exit(0);
        }


        private void ConsoleLoop()
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

    }

}
