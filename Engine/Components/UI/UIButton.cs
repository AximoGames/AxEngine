// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Engine.Windows;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Aximo.Engine.Components.UI
{

    public class UIButton : UIContainerComponent
    {
        public UILabelComponent LabelComponent;

        public UIButton()
        {
            Padding = new UIAnchors(10, 10, 10, 10);
            Border = new UIAnchors(1);
            LabelComponent = new UILabelComponent();
            AddComponent(LabelComponent);
        }

        public string Text
        {
            get => LabelComponent.Text;
            set => LabelComponent.Text = value;
        }

        public Color Color
        {
            get => LabelComponent.Color;
            set => LabelComponent.Color = value;
        }

        public float FontSize
        {
            get => LabelComponent.FontSize;
            set => LabelComponent.FontSize = value;
        }

        public Color BackColor { get; set; } = Color.Gray;
        public Color BackColorHover { get; set; } = Color.LightGray;

        public Color BorderColor { get; set; } = Color.Black;
        public Color BorderColorHover { get; set; } = Color.Black;
        public float BorderRadius { get; set; } = 10;

        protected override void DrawControl()
        {
            Color bgColor;
            Color borderColor;

            if (MouseEntered)
            {
                bgColor = BackColorHover;
                borderColor = BorderColorHover;
            }
            else
            {
                bgColor = BackColor;
                borderColor = BorderColor;
            }

            ImageContext.Clear(bgColor);
            ImageContext.DrawButton(Border, borderColor, BorderRadius);
        }

        public override void OnMouseEnter(MouseMoveArgs e)
        {
            InvokeDrawControl();
        }

        public override void OnMouseLeave(MouseMoveArgs e)
        {
            InvokeDrawControl();
        }
    }
}
