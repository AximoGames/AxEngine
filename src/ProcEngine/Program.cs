using System;
using System.Threading;
using OpenTK;

namespace ProcEngine
{
    class MainClass
    {

        private static Thread th;

        public static void Main(string[] args)
        {
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
                var args = cmd.Split(' ', StringSplitOptions.RemoveEmptyEntries);
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

        private static Window demo;

        private static void UIThreadMain()
        {
            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferX11,
            });

            demo = new Window(800, 600, "ProcEngine");
            demo.Location = new System.Drawing.Point(1920 / 2 + 10, 10);

            // Run the game at 60 updates per second
            demo.Run(60.0);
        }

    }
}
