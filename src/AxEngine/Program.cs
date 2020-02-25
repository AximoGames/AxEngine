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

        private static Window demo;
        private static DebugProc _debugProcCallback = DebugCallback;
        private static GCHandle _debugProcCallbackHandle;

        private static void UIThreadMain()
        {
            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferX11,
            });

            // _debugProcCallbackHandle = GCHandle.Alloc(_debugProcCallback);
            // GL.DebugMessageCallback(_debugProcCallback, IntPtr.Zero);
            // GL.Enable(EnableCap.DebugOutput);
            // GL.Enable(EnableCap.DebugOutputSynchronous);

            demo = new Window(800, 600, "AxEngine");
            demo.Location = new System.Drawing.Point(1920 / 2 + 10, 10);

            // Run the game at 60 updates per second
            demo.Run(60.0);
        }

        public static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            Console.WriteLine($"{severity} {type} | {messageString}");

            if (type == DebugType.DebugTypeError)
            {
                throw new Exception(messageString);
            }
        }
    }
}
