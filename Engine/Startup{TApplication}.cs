// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Windows;
using Aximo.Render.OpenGL;

namespace Aximo.Engine
{

    /// <inheritdoc/>
    public class Startup<TApplication> : Startup
        where TApplication : Application, new()
    {
        private ApplicationConfig Config;

        public Startup() : this(new ApplicationConfig())
        {
        }

        public Startup(ApplicationConfig config)
        {
            Config = config;
        }

        private static Serilog.ILogger Log = Aximo.Log.ForContext<Startup<TApplication>>();

        private Thread th;

        private GtkUI ui;

        private protected virtual GtkUI CreateGtkUI() => new GtkUI();

        public void Start()
        {
            Start(null);
        }

        internal void Start(TApplication instance)
        {
            Application = instance;
            var bits = IntPtr.Size == 4 ? 32 : 64;
            Log.Verbose($"{bits} Bit System detected. (Pointer Size: {IntPtr.Size} Bytes)");
            Log.Verbose("OS: {OSVersion}", Environment.OSVersion);

            if (Config.UseGtkUI)
            {
                Log.Verbose("Init GtkUI");
                ui = CreateGtkUI();
                ui.Start();
                Log.Verbose("GtkUI initialized");
            }

            if (Config.UseConsole)
            {
                Log.Verbose("Configure for console session");
                th = new Thread(UIThreadMain);
                th.Start();

                ConsoleLoop();

                Application.Stop();
                Application.Dispose();
                th.Abort();
                Environment.Exit(0);
            }
            else
            {
                UIThreadMain();
                Environment.Exit(0);
            }
        }

        private Application Application;

        private void UIThreadMain()
        {
            if (Application == null)
                Application = Activator.CreateInstance<TApplication>();

            Application.SetConfig(Config);
            Application.RunSync();
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
                    case "reload-plugins":
                        PluginManager.Reload();
                        break;
                    case "act-disable-all":
                        SceneManager.GetCurrentScene().GetActors().ForEach(act => act.SetActive(false));
                        break;
                    case "act-enable-all":
                        SceneManager.GetCurrentScene().GetActors().ForEach(act => act.SetActive(true));
                        break;

                    //case "diag":
                    //    SceneContext.Current.DumpInfo(true);
                    //    break;
                    //case "actor new":
                    //    Application.Current.DispatchUpdater(() =>
                    //    {
                    //        SceneContext.Current.AddActor(new Actor(new CubeComponent()));
                    //    });
                    //    break;
                    //case "act del":
                    //case "actor del":
                    //    Application.Current.DispatchUpdater(() =>
                    //    {
                    //        SceneContext.Current.RemoveActor(SceneContext.Current.Actors.LastOrDefault());
                    //    });
                    //    break;
                    //case "map list":
                    //    Application.Current.DispatchUpdater(() =>
                    //    {
                    //        InternalTextureManager.DumpInfo(true);
                    //    });
                    //    break;
                    default:
                        var a = new ExecuteConsoleCommandLineArgs(cmd);
                        CommandLineManager.Current.InvokeExecuteConsoleCommandLine(a);
                        if (!a.Handled)
                            Console.WriteLine("Unknown command");
                        break;
                }
            }
        }

        private bool disposedValue = false;
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Application.Dispose();
                    ui.Dispose();
                }
                Application = null;
                ui = null;

                disposedValue = true;
            }
        }
    }
}
