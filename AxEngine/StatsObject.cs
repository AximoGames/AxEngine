// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using Aximo.Render;

namespace Aximo.Engine
{
    public class StatsObject : ScreenTextureObject, IUpdateFrame
    {
        private GraphicsTexture GfxTexture;
        private DateTime LastStatUpdate;
        private Font DefaultFont = new Font(FontFamily.GenericSansSerif, 15, GraphicsUnit.Point);

        public StatsObject()
        {
        }

        public override void Init()
        {
            GfxTexture = new GraphicsTexture(100, 100);
            GfxTexture.Graphics.Clear(Color.Green);
            SourceTexture = GfxTexture.Texture;
            GfxTexture.UpdateTexture();
            base.Init();
        }

        public void OnUpdateFrame()
        {
            if ((DateTime.UtcNow - LastStatUpdate).TotalSeconds > 1)
            {
                LastStatUpdate = DateTime.UtcNow;
                GfxTexture.Graphics.Clear(Color.Transparent);
                var txt = "FPS: " + Math.Round(RenderApplication.Current.RenderFrequency).ToString();
                txt += "\nUPS: " + Math.Round(RenderApplication.Current.UpdateFrequency).ToString();
                GfxTexture.Graphics.DrawString(txt, DefaultFont, Brushes.White, new PointF(5, 5));
                GfxTexture.UpdateTexture();
            }
        }
    }

}
