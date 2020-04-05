// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Render;
using OpenToolkit;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.Shapes;
using Color = SixLabors.ImageSharp.Color;
using PointF = SixLabors.Primitives.PointF;

namespace Aximo.Engine
{

    public class StatsComponent : GraphicsScreenTextureComponent
    {
        private DateTime LastStatUpdate;
        private Font DefaultFont = new Font(SystemFonts.Families.First(), 15, FontStyle.Regular);

        public StatsComponent() : base(100, 100)
        {
        }

        public StatsComponent(int width, int height) : base(width, height)
        {
            Image.Mutate(ctx => ctx.Fill(Color.Green));
            UpdateTexture();
        }

        public override void UpdateFrame()
        {
            if ((DateTime.UtcNow - LastStatUpdate).TotalSeconds > 1)
            {
                LastStatUpdate = DateTime.UtcNow;
                Image.Mutate(ctx => ctx.Fill(Color.Green));
                var txt = "FPS: " + Math.Round(RenderApplication.Current.RenderCounter.EventsPerSecond).ToString();
                txt += "\nUPS: " + Math.Round(RenderApplication.Current.UpdateCounter.EventsPerSecond).ToString();
                Image.Mutate(ctx => ctx.DrawText(txt, DefaultFont, Color.White, new PointF(5, 5)));
                UpdateTexture();
            }
        }

    }

}
