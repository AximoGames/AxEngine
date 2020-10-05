// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aximo.Engine.Windows;
using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public class SceneContext : SceneObject
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<SceneContext>();

        public float ReferenceScreenWidth { get; private set; } = 3840f;
        public Vector2 PixelToScaleFactor { get; private set; }
        public Vector2 ScaleToPixelFactor { get; private set; }
        public Vector2 ScreenScale { get; set; } = Vector2.One;
        public Vector2i ScreenPixelSize => RenderContext.ScreenPixelSize;
        public Vector2 ScreenScaledSize => ScreenPixelSize.ToVector2() * ScreenScale;

        public static SceneContext Current { get; set; }

        public static bool IsRenderThread => WindowContext.IsRenderThread;
        public static bool IsUpdateThread => WindowContext.IsUpdateThread;

        public List<IUpdateFrame> UpdateFrameObjects = new List<IUpdateFrame>();

        /// <summary>
        /// Scalable Clock
        /// </summary>
        public readonly Clock Clock = new Clock();

        public void OnUpdateFrame()
        {
        }

        public void Sync()
        {
        }

        private RenderContext RenderContext => RenderContext.Current;

        public void Init()
        {
            //SetScale();
            //EngineAssets.Init();
        }

        private void SetScale()
        {
            ScreenScale = Application.Current.GetScreenPixelScale();
            PixelToScaleFactor = ScreenScale;
            ScaleToPixelFactor = Vector2.Divide(Vector2.One, ScreenScale);
        }

        public TimeSpan Time { get; set; }

        internal void UpdateTime()
        {
            Clock.Tick(Application.Current.UpdateCounter.Elapsed.TotalSeconds);
        }

        public void OnScreenResize(ScreenResizeEventArgs e)
        {
            SetScale();
            //Visit<Actor>(c => c.OnScreenResize(e));
        }

        public void OnScreenMouseUp(MouseButtonArgs e)
        {
            //Visit<Actor>(c => c.OnScreenMouseUp(e), c => !e.Handled);
        }

        public void OnScreenMouseDown(MouseButtonArgs e)
        {
            //Visit<Actor>(c => c.OnScreenMouseDown(e), c => !e.Handled);
        }

        public void OnScreenMouseMove(MouseMoveArgs e)
        {
            //Visit<Actor>(c => c.OnScreenMouseMove(e), c => !e.Handled);
        }
    }
}
