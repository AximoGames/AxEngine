// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Engine;
using OpenToolkit.Windowing.GraphicsLibraryFramework;

namespace Aximo.Engine
{
    public class GameStartup<TApp, TGtk> : IDisposable
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
            var bits = IntPtr.Size == 4 ? 32 : 64;
            Log.Verbose($"{bits} Bit System detected. (Pointer Size: {IntPtr.Size} Bytes)");
            Log.Verbose("OS: {OSVersion}", Environment.OSVersion);

            if (Config.UseGtkUI)
            {
                Log.Verbose("Init GtkUI");
                ui = new GtkUI();
                ui.Start();
                Log.Verbose("GtkUI initialized");
            }

            if (Config.UseConsole)
            {
                Log.Verbose("Configure for console session");
                th = new Thread(UIThreadMain);
                th.Start();

                ConsoleLoop();

                demo.Stop();
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
            demo.RunSync();
            Environment.Exit(0);
        }

        private void ConsoleLoop()
        {
            string prevCmd = "";
            while (true)
            {
                var cmd = Console.ReadLine().Trim();
                if (cmd == "r")
                    cmd = prevCmd;
                else
                    prevCmd = cmd;

                var args = cmd.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length == 0)
                    continue;

                switch (cmd)
                {
                    case "q":
                        return;
                    case "diag":
                        GameContext.Current.DumpInfo(true);
                        break;
                    case "actor new":
                        RenderApplication.Current.DispatchUpdater(() =>
                        {
                            GameContext.Current.AddActor(new Actor(new CubeComponent()));
                        });
                        break;
                    case "act del":
                    case "actor del":
                        RenderApplication.Current.DispatchUpdater(() =>
                        {
                            GameContext.Current.RemoveActor(GameContext.Current.Actors.LastOrDefault());
                        });
                        break;
                    case "map list":
                        RenderApplication.Current.DispatchUpdater(() =>
                        {
                            Render.InternalTextureManager.DumpInfo(true);
                        });
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    demo.Dispose();
                    ui.Dispose();
                }
                demo = null;
                ui = null;

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
