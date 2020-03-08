using System;
using OpenTK.Graphics.OpenGL4;
using System.Threading;
using OpenTK;
using System.Runtime.InteropServices;

namespace AxEngine
{
    class MainClass
    {

        private static Thread th;

        public static void Main(string[] args)
        {
            var bits = IntPtr.Size == 4 ? 32 : 64;
            Console.WriteLine($"{bits} Bit System detected. (Pointer Size: {IntPtr.Size} Bytes)");
            Console.WriteLine($"OS: {Environment.OSVersion.ToString()}");

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
            demo = new RenderDemo(new RenderApplicationStartup
            {
                WindowTitle = "AxEngineDemo",
                WindowSize = new Vector2i(800, 600),
                WindowBorder = WindowBorder.Fixed,
            });
            demo.Run();
        }

    }
}
