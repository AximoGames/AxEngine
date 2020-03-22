// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Render;
using OpenTK;

namespace Aximo.Engine
{

    public class StatsComponent : GraphicsScreenTextureComponent
    {
        private DateTime LastStatUpdate;
        private Font DefaultFont = new Font(FontFamily.GenericSansSerif, 15, GraphicsUnit.Point);

        public StatsComponent() : base(100, 100)
        {
        }

        public StatsComponent(int width, int height) : base(width, height)
        {
            Graphics.Clear(Color.Green);
            UpdateTexture();
        }

        public override void UpdateFrame()
        {
            if ((DateTime.UtcNow - LastStatUpdate).TotalSeconds > 1)
            {
                LastStatUpdate = DateTime.UtcNow;
                Graphics.Clear(Color.Transparent);
                var txt = "FPS: " + Math.Round(RenderApplication.Current.RenderFrequency).ToString();
                txt += "\nUPS: " + Math.Round(RenderApplication.Current.UpdateFrequency).ToString();
                Graphics.DrawString(txt, DefaultFont, Brushes.White, new PointF(5, 5));
                UpdateTexture();
            }
        }

    }

}
