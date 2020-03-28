// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using System.Threading;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Aximo.Engine;
using OpenTK;
using Xunit;
using System.IO;
using Aximo.Render;

namespace Aximo.AxDemo
{

    public class RenderApplicationTests : RenderApplication
    {
        protected Thread UpdaterThread;
        private AutoResetEvent SetupWaiter;
        public RenderApplicationTests() : base(new RenderApplicationStartup
        {
            WindowTitle = "AxTests",
            WindowSize = new Vector2i(160, 120),
            WindowBorder = WindowBorder.Fixed,
        })
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

        private BufferComponent ScreenshotBuffer;

        protected override void SetupScene()
        {
            GameContext.BackgroundColor = new Vector4(0.2f, 0.3f, 0.3f, 1);
            GameContext.AddActor(new Actor(ScreenshotBuffer = new BufferComponent()));
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

        private string TestOutputDir => Path.Combine(DirectoryHelper.GetAssetsPath("TestOutputs"), GetType().Name);
        public bool OverwriteOriginalImages;

        protected void RenderAndCompare(string testName)
        {
            RenderSingleFrameSync();
            var bmpCurrent = ScreenshotBuffer.BufferData.CreateBitmap();

            var testCaseFilePrefix = Path.Combine(TestOutputDir, testName);
            Directory.CreateDirectory(TestOutputDir);
            var originalFile = testCaseFilePrefix + ".original.png";
            var currentFile = testCaseFilePrefix + ".current.png";

            if (!File.Exists(originalFile) || OverwriteOriginalImages)
            {
                bmpCurrent.Save(originalFile);

                if (File.Exists(currentFile))
                    File.Delete(currentFile);
                return;
            }

            var bmpOriginal = Bitmap.FromFile(originalFile);

            var maxDiffAllowed = 1000;

            var diff = CompareImage(bmpCurrent, bmpOriginal, maxDiffAllowed);
            if (diff > maxDiffAllowed)
            {
                bmpCurrent.Save(currentFile);
                Console.WriteLine($"MaxDifference: {diff} MaxDiffAllowed: {maxDiffAllowed}");
            }
            Assert.InRange(diff, 0, maxDiffAllowed);
        }

        protected int CompareImage(Image img1, Image img2, int maxDiffAllowed)
        {

            if (img1 == null || img2 == null | img1.Width != img2.Width || img1.Height != img2.Height)
                return -1;

            var bmp1 = ResizeImage(img1);
            var bmp2 = ResizeImage(img2);
            var maxDiff = Analyzse(bmp1, bmp2, maxDiffAllowed);

            if (maxDiff > maxDiffAllowed)
            {
                Analyzse(bmp1, bmp2, maxDiffAllowed, true);
            }

            return maxDiff;
        }

        private int Analyzse(Bitmap bmp1, Bitmap bmp2, int maxDiffAllowed, bool showChanges = false)
        {
            int maxDiff = 0;

            if (showChanges)
                Console.WriteLine();

            for (var y = 0; y < bmp1.Height; y++)
            {
                for (var x = 0; x < bmp1.Width; x++)
                {
                    var dist = GetDistanceBetweenColours(bmp1.GetPixel(x, y), bmp2.GetPixel(x, y));

                    if (showChanges)
                        Console.Write(dist > maxDiffAllowed ? "#" : ".");

                    maxDiff = Math.Max(maxDiff, dist);
                }

                if (showChanges)
                    Console.WriteLine();
            }
            return maxDiff;
        }

        private static int GetDistanceBetweenColours(Color a, Color b)
        {
            int dR = a.R - b.R, dG = a.G - b.G, dB = a.B - b.B;
            return dR * dR + dG * dG + dB * dB;
        }

        private Bitmap ResizeImage(Image image)
        {
            var pixelBlock = 3;
            int newWidth = (int)Math.Round(image.Width / (double)pixelBlock);
            int newHeight = (int)Math.Round((double)image.Height / (double)pixelBlock);
            Bitmap squeezed = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppRgb);
            Graphics canvas = Graphics.FromImage(squeezed);
            canvas.CompositingQuality = CompositingQuality.HighQuality;
            canvas.InterpolationMode = InterpolationMode.HighQualityBilinear;
            canvas.SmoothingMode = SmoothingMode.HighQuality;
            canvas.DrawImage(image, 0, 0, newWidth, newHeight);
            canvas.Flush();
            canvas.Dispose();
            return squeezed;
        }

        protected GameMaterial GetTestMaterial(PipelineType pipelineType, Vector3 color)
        {
            return new GameMaterial()
            {
                DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png"),
                SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
                Ambient = 1f,
                ColorBlendMode = MaterialColorBlendMode.Set,
                Color = color,
                PipelineType = pipelineType,
            };
        }

        protected Transform GetTestTransform()
        {
            return new Transform
            {
                Scale = new Vector3(1),
                Rotation = new Vector3(0, 0, 0.5f).ToQuaternion(),
                Translation = new Vector3(0f, 0, 0.5f),
            };
        }

    }

}
