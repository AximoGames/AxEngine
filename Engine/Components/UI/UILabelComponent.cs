// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using System.Linq;

namespace Aximo.Engine
{
    public class UILabelComponent : UIComponent
    {
        public string Text { get; set; }
        private Font DefaultFont = new Font(SystemFonts.Families.First(), 30, FontStyle.Regular);

        public UILabelComponent()
        {
        }

        public UILabelComponent(string text)
        {
            Text = text;
            Name = "Label";
        }

        protected override void DrawControl()
        {
            var options = new TextGraphicsOptions
            {
                VerticalAlignment = VerticalAlignment.Center,
            };
            Image.Mutate(ctx => ctx.DrawText(options, "blubb", DefaultFont, Color.Red, new PointF(0, RelatativePaddingRect.Size.Y / 2)));
        }

        protected override void OnResized()
        {
            InvokeDrawControl();
        }

    }

}
