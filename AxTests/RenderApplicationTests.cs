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
        protected Thread UpdaterThread;
        private AutoResetEvent SetupWaiter;
        public RenderApplicationTests(RenderApplicationStartup startup) : base(startup)
        {
            DebugHelper.LogThreadInfo("UnitTestThread");
            SetupWaiter = new AutoResetEvent(false);
            AfterApplicationInitialized += () => SetupWaiter.Set();
            UpdaterThread = new Thread(() =>
            {
                try
                {
                    Run();
                }
                catch (Exception ex)
                {
                    if (!Exiting)
                        throw;
                }
            });
            UpdaterThread.Start();
            SetupWaiter.WaitOne(4000);
            Console.WriteLine("Ready for tests");
        }

        protected override void SetupScene()
        {
            GameContext.BackgroundColor = new Vector3(1, 0, 1);
        }

        private float LightAngle = 0;

        protected override void BeforeUpdateFrame()
        {
            if (Exiting)
                return;

            if (UpdateFrameNumber == 0)
                UpdateWaiter.WaitOne();
            else
                WaitHandle.SignalAndWait(RenderWaiter, UpdateWaiter);
        }

        protected override void BeforeRenderFrame()
        {
            if (Exiting)
                return;
            if (RenderFrameNumber == 0)
                RenderWaiter.WaitOne();
            else
                WaitHandle.SignalAndWait(TestWaiter, RenderWaiter);
        }

        public bool RendererEnabled;
        public AutoResetEvent UpdateWaiter = new AutoResetEvent(false);
        public AutoResetEvent RenderWaiter = new AutoResetEvent(false);
        public AutoResetEvent TestWaiter = new AutoResetEvent(false);

        public void RenderSingleFrameSync()
        {
            Console.WriteLine(" --- Render Single Frame ---");
            WaitHandle.SignalAndWait(UpdateWaiter, TestWaiter);
        }

        public override void Dispose()
        {
            SignalShutdown();
            Console.WriteLine("Shutting down Test");
            TestWaiter.Set();
            RenderWaiter.Set();
            UpdateWaiter.Set();

            base.Dispose();

            UpdateWaiter.Dispose();
            UpdateWaiter = null;
            RenderWaiter.Dispose();
            RenderWaiter = null;
            TestWaiter.Dispose();
            TestWaiter = null;
        }

    }

}
