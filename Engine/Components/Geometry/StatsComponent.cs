// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Aximo.Engine.Components.UI;
using OpenToolkit.Mathematics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;

namespace Aximo.Engine.Components.Geometry
{
    /// <summary>
    /// Displays metrics, for example the Frames per Second and Updates per Second.
    /// </summary>
    public class StatsComponent : UIComponent
    {
        private DateTime LastStatUpdate;

        public StatsComponent() : this(new Vector2(200, 100))
        {
        }

        public StatsComponent(Vector2 size) : base(size)
        {
            ImageContext.Clear(Color.Transparent);
            UpdateTexture();
        }

        public float FontSize = 24;

        public override void UpdateFrame()
        {
            if ((DateTime.UtcNow - LastStatUpdate).TotalSeconds > 1)
            {
                LastStatUpdate = DateTime.UtcNow;
                ImageContext.Clear(Color.Transparent);
                var txt = "FPS: " + Math.Round(Application.Current.RenderCounter.EventsPerSecond).ToString();
                txt += "\nUPS: " + Math.Round(Application.Current.UpdateCounter.EventsPerSecond).ToString();
                ImageContext.FontSize = FontSize;
                ImageContext.FillStyle(Color.White);
                ImageContext.DrawText(txt, new Vector2(5, 5));
                UpdateTexture();
            }
            base.UpdateFrame();
        }
    }
}
