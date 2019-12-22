using LearnOpenTK;
using System;

namespace Net3dBoolDemo
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var demo = new TDemoWindow();
            //var demo = new Window(800, 600, "blubb");

            // Run the game at 60 updates per second
            demo.Run(60.0);
        }
    }
}
