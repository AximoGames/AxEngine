// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Aximo.Engine;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;
using Xunit;

namespace Aximo.AxTests
{

    public class RenderApplicationTests : RenderApplication
    {

        // We need to ensure, that every unit test uses the same UI Thread,
        // otherwise GLFW will fail.
        protected static Thread UpdaterThread;

        protected static RenderApplicationTests CurrentTestApp;

        private static AutoResetEvent UpdaterThreadNextTaskWaiter;
        protected static void UpdaterThreadMain()
        {
            while (true)
            {
                UpdaterThreadNextTaskWaiter.WaitOne();
                try
                {
                    CurrentTestApp.Run();
                }
                catch (Exception ex)
                {
                    if (!CurrentTestApp.Exiting)
                        Console.WriteLine(ex);
                }
            }
        }

        private AutoResetEvent SetupWaiter;
        public RenderApplicationTests() : base(new RenderApplicationConfig
        {
            WindowTitle = "AxTests",
            WindowSize = new Vector2i(160, 120),
            WindowBorder = WindowBorder.Fixed,
            HideTitleBar = true,
        })
        {
            DebugHelper.LogThreadInfo("UnitTestThread");
            SetupWaiter = new AutoResetEvent(false);
            CurrentTestApp = this;
            AfterApplicationInitialized += () =>
            {
                SetupWaiter.Set();
            };
            if (UpdaterThread == null)
            {
                UpdaterThreadNextTaskWaiter = new AutoResetEvent(false);
                UpdaterThread = new Thread(UpdaterThreadMain);
                UpdaterThread.Start();
            }
            UpdaterThreadNextTaskWaiter.Set();
            SetupWaiter.WaitOne();
            //SetupWaiter.WaitOne(4000);
            SetupWaiter.Dispose();
            SetupWaiter = null;
            Console.WriteLine("Ready for tests");
        }

        public BufferComponent ScreenshotBuffer;

        protected override void SetupScene()
        {
            GameContext.BackgroundColor = new Vector4(0.2f, 0.3f, 0.3f, 1);
            GameContext.AddActor(new Actor(ScreenshotBuffer = new BufferComponent()));
        }

        protected override void BeforeUpdateFrame()
        {
            if (Exiting)
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

            if (Exiting)
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

        public override void Dispose()
        {
            SignalShutdown();
            Console.WriteLine("Shutting down Test");
            ScreenshotBuffer?.Dispose();
            TestWaiter.Set();
            RenderWaiter.Set();
            UpdateWaiter.Set();

            SetupWaiter?.Dispose();
            SetupWaiter = null;

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
