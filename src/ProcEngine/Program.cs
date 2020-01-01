using LearnOpenTK;
using System;

namespace ProcEngine
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //var demo = new TDemoWindow();
            var demo = new Window(800, 600, "blubb");
            //var demo = new Win2(800, 600, "blubb");

            // Run the game at 60 updates per second
            demo.Run(60.0);
        }
    }
}
