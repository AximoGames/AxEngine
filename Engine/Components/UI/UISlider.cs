// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using Aximo.Engine.Windows;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Aximo.Engine.Components.UI
{

    public enum Orientation
    {
        Horizontal,
        Vertical,
    }

    public class UISlider : UIContainerComponent
    {



        public class UISliderButton : UIButton
        {
            public UISliderButton()
            {
                Padding = new UIAnchors();
                Margin = new UIAnchors();

                MinValue = 0;
                MaxValue = 100;
                Value = 0;
            }

            public float _ProgressFactor;
            public float ProgressFactor
            {
                get => _ProgressFactor;
                set
                {
                    Value = value / ValueToProgressFactor;
                }
            }

            public float _Value;
            public float Value
            {
                get => _Value;
                set
                {
                    if (value == _Value)
                        return;
                    _Value = Math.Clamp(value, MinValue, MaxValue);
                    _ProgressFactor = Math.Clamp(_Value * ValueToProgressFactor, 0, 1);
                }
            }

            public float MinValue { get; set; }
            public float MaxValue { get; set; }

            public float SliderThickness { get; set; } = 20;

            private Box2 ParentRect => ((UIComponent)Parent).AbsolutePaddingRect;
            private Vector2 SliderSize => SwapAxis(new Vector2(SliderThickness, SwapAxis(ParentRect.Size).Y));
            private float PixelRange => SwapAxis(ParentRect.Size).X - SwapAxis(SliderSize).X;
            private float PixelToProgressFactor => 1 / PixelRange;
            private float ValueToProgressFactor => 1 / (MaxValue - MinValue);

            public Orientation Orientation { get; set; } = Orientation.Vertical;
            private Vector2 SwapAxis(Vector2 vec) => Orientation == Orientation.Horizontal ? vec : new Vector2(vec.Y, vec.X);

            internal override void SetComponentSize()
            {
                Size = SliderSize;
                var xPos = PixelRange * ProgressFactor;
                Location = SwapAxis(new Vector2(xPos, SwapAxis(Location).Y));

                base.SetComponentSize();
            }

            private Vector2 StartPosition;
            private float StartProgress;
            private bool InMovement;
            public override void OnMouseDown(MouseButtonArgs e)
            {
                InMovement = true;
                StartPosition = e.Position;
                StartProgress = ProgressFactor;
                base.OnMouseDown(e);
            }

            public override void OnScreenMouseUp(MouseButtonArgs e)
            {
                base.OnScreenMouseUp(e);
                if (!InMovement)
                    return;
                InMovement = false;
            }

            public override void OnScreenMouseMove(MouseMoveArgs e)
            {
                base.OnScreenMouseMove(e);

                if (!InMovement)
                    return;

                var diffPixel = e.Position - StartPosition;
                var progressDiff = SwapAxis(diffPixel).X * PixelToProgressFactor;
                ProgressFactor = StartProgress + progressDiff;
            }
        }

        public UISliderButton Button;

        public UISlider()
        {
            Padding = new UIAnchors(10, 10, 10, 10);
            Button = new UISliderButton();
            AddComponent(Button);
            BackColor = new Color(new System.Numerics.Vector4(0, 1, 0, 0.5f));
            BackColorHover = new Color(new System.Numerics.Vector4(1, 1, 0, 0.5f));
        }

        internal override void SetChildBounds()
        {
            base.SetChildBounds();
        }

        public Color BackColor { get; set; } = Color.Gray;
        public Color BackColorHover { get; set; } = Color.LightGray;

        public Color BorderColor { get; set; } = Color.Black;
        public Color BorderColorHover { get; set; } = Color.Black;

        public float BorderSize { get; set; } = 1;

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

            Image.Mutate(ctx => ctx.Clear(bgColor));
            Image.Mutate(ctx => ctx.DrawButton(BorderSize, borderColor, 10f));
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
            base.OnResized();
        }
    }
}
