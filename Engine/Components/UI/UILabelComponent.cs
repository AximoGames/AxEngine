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
        private static FontFamily DefaultFamily = SystemFonts.Families.First();
        public float FontSize { get; set; } = 15;

        public UILabelComponent()
        {
        }

        public UILabelComponent(string text)
        {
            Text = text;
            Name = "Label";
        }

        public Color Color { get; set; } = Color.Black;

        protected override void DrawControl()
        {
            var options = new TextGraphicsOptions
            {
                VerticalAlignment = VerticalAlignment.Center,
            };
            if (Text != null)
                Image.Mutate(ctx => ctx.DrawText(options, Text, new Font(DefaultFamily, FontSize, FontStyle.Regular), Color, new PointF(0, RelatativePaddingRect.Size.Y / 2)));
        }

        protected override void OnResized()
        {
            InvokeDrawControl();
        }

        internal override void DumpInfo(bool list)
        {
            Log.ForContext("DumpInfo").Info(new string(' ', (Level + 1) * 2) + "{Type} #{Id} {Name} Text={Text}", GetType().Name, ObjectId, Name, Text);
            if (list)
                VisitChilds<GameObject>(a => a.DumpInfo(false));
        }

    }

}
