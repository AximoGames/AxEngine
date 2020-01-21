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

            //var demo = new TDemoWindow();
            var demo = new Window(800, 600, "blubb");
            //var demo = new Win2(800, 600, "blubb");

            // Run the game at 60 updates per second
            demo.Run(60.0);
        }
    }
}
