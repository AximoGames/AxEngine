using System;
using OpenTK;

namespace ProcEngine
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferX11,
            });

            var demo = new Window(800, 600, "ProcEngine");

            // Run the game at 60 updates per second
            demo.Run(60.0);
        }
    }
}
