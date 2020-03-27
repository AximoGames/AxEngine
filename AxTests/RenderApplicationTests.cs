// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using System.Threading;
using Aximo.Engine;
using OpenTK;

namespace Aximo.AxDemo
{

    public class RenderApplicationTests : RenderApplication
    {
        private Thread thread;
        private AutoResetEvent waiter;
        public RenderApplicationTests(RenderApplicationStartup startup) : base(startup)
        {
            waiter = new AutoResetEvent(false);
            AfterApplicationInitialized += () => waiter.Set();
            thread = new Thread(() => Run());
            thread.Start();
            waiter.WaitOne(10000);
            Console.WriteLine("Ready for tests");
        }

        protected override void SetupScene()
        {
            GameContext.BackgroundColor = new Vector3(1, 0, 1);
        }

        private float LightAngle = 0;

        protected override void OnRenderFrame(FrameEventArgs e)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
        }

    }

}
