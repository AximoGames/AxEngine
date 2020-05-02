// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using Color = SixLabors.ImageSharp.Color;

namespace Aximo.Engine
{
    public class StatsComponent : UIComponent
    {
        private DateTime LastStatUpdate;
        private Font DefaultFont = new Font(SystemFonts.Families.First(), 12, FontStyle.Regular);

        public StatsComponent() : this(new Vector2i(200, 100))
        {
        }

        public StatsComponent(Vector2i size) : base(size)
        {
            Image.Mutate(ctx => ctx.Clear(Color.Transparent));
            UpdateTexture();
        }

        public override void UpdateFrame()
        {
            if ((DateTime.UtcNow - LastStatUpdate).TotalSeconds > 1)
            {
                LastStatUpdate = DateTime.UtcNow;
                Image.Mutate(ctx => ctx.Clear(Color.Transparent));
                var txt = "FPS: " + Math.Round(RenderApplication.Current.RenderCounter.EventsPerSecond).ToString();
                txt += "\nUPS: " + Math.Round(RenderApplication.Current.UpdateCounter.EventsPerSecond).ToString();
                Image.Mutate(ctx => ctx.DrawText(txt, DefaultFont, Color.White, new PointF(5, 5)));
                UpdateTexture();
            }
            base.UpdateFrame();
        }
    }
}
