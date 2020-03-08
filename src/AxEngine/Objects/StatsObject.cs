using OpenToolkit.Mathematics;
using OpenToolkit.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AxEngine
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
            SourceTexture = GfxTexture.Texture;
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
