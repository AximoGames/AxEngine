// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Aximo.Engine;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo.AxTests
{
    public class TestsApplication : Application
    {
        protected static TestsApplication CurrentTestApp;

        public TestsApplication()
        {
            SetConfig(new ApplicationConfig
            {
                WindowTitle = "AxTests",
                WindowSize = new Vector2i(160, 120),
                WindowBorder = WindowBorder.Fixed,
                HideTitleBar = true,
            });
        }

        internal override void Run()
        {
            CurrentTestApp = this;
            AfterApplicationInitialized += () =>
            {
            };
            base.Run();
        }

        public BufferComponent ScreenshotBuffer;

        protected override void SetupScene()
        {
            SceneContext.BackgroundColor = new Vector4(0.2f, 0.3f, 0.3f, 1);
            SceneContext.AddActor(new Actor(ScreenshotBuffer = new BufferComponent()));
        }

        protected override void BeforeUpdateFrame()
        {
            if (Closing)
                return;

            if (IsMultiThreaded)
            {
                if (UpdateFrameNumber == 0)
                    UpdateWaiter.WaitOne();
                else
                    WaitHandle.SignalAndWait(RenderWaiter, UpdateWaiter);
            }
            else
            {
                if (WaitForRenderer)
                    return;

                if (UpdateFrameNumber == 0)
                {
                    UpdateWaiter.WaitOne();
                }
                else
                {
                    WaitHandle.SignalAndWait(TestWaiter, UpdateWaiter);
                }
            }
        }

        private bool WaitForRenderer = false;

        protected override void BeforeRenderFrame()
        {
            WaitForRenderer = false;

            if (Closing)
                return;

            if (IsMultiThreaded)
            {
                if (RenderFrameNumber == 0)
                    RenderWaiter.WaitOne();
                else
                    WaitHandle.SignalAndWait(TestWaiter, RenderWaiter);
            }
        }

        public bool RendererEnabled;
        public AutoResetEvent UpdateWaiter = new AutoResetEvent(false);
        public AutoResetEvent RenderWaiter = new AutoResetEvent(false);
        public AutoResetEvent TestWaiter = new AutoResetEvent(false);

        public void RenderSingleFrameSync()
        {
            Console.WriteLine(" --- Render Single Frame ---");
            WaitForRenderer = true;
            WaitHandle.SignalAndWait(UpdateWaiter, TestWaiter);
        }

        protected override void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            SignalShutdown();
            Console.WriteLine("Shutting down Test");
            ScreenshotBuffer?.Dispose();
            TestWaiter.Set();
            RenderWaiter.Set();
            UpdateWaiter.Set();

            base.Dispose(disposing);

            UpdateWaiter.Dispose();
            UpdateWaiter = null;
            RenderWaiter.Dispose();
            RenderWaiter = null;
            TestWaiter.Dispose();
            TestWaiter = null;
        }
    }
}
