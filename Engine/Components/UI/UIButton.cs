// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;

namespace Aximo.Engine
{
    public class UIButton : UIContainerComponent
    {

        public UILabelComponent LabelComponent;

        public UIButton()
        {
            LabelComponent = new UILabelComponent();
            AddComponent(LabelComponent);
        }

        protected override void DrawControl()
        {
            var bgNormal = Color.Gray;
            var bgHover = Color.DarkGray;

            Color bgColor;

            if (MouseEntered)
            {
                bgColor = bgHover;
            }
            else
            {
                bgColor = bgNormal;
            }

            Image.Mutate(ctx => ctx.Clear(bgColor));
            Image.Mutate(ctx => ctx.DrawButton(5f, Color.Black, 10f));
        }

        public override void OnMouseEnter(MouseMoveArgs e)
        {
            InvokeDrawControl();
        }

        public override void OnMouseLeave(MouseMoveArgs e)
        {
            InvokeDrawControl();
        }

        protected override void OnResized()
        {
            var s = "";
        }

    }

}
