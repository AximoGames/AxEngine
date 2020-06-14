// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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

    public class SliderValueChangedArgs
    {
        public float OldValue;
        public float NewValue;
        public UISlider.UISliderButton Button;
    }

    public delegate void SliderValueChanged(SliderValueChangedArgs e);

    public class UISlider : UIContainerComponent
    {

        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float SliderThickness { get; set; } = 20;
        public Orientation Orientation { get; set; } = Orientation.Horizontal;

        public event SliderValueChanged SliderValueChanged;

        public float Progress
        {
            get => Button.Progress;
            set => Button.Progress = value;
        }

        public float Value
        {
            get => Button.Value;
            set => Button.Value = value;
        }

        internal void OnSliderValueChangedInternal(SliderValueChangedArgs e)
        {
            if (Parent == null)
                return;

            OnSliderValueChanged(e);
        }

        protected virtual void OnSliderValueChanged(SliderValueChangedArgs e)
        {
            SliderValueChanged?.Invoke(e);
        }

        public class UISliderButton : UIButton
        {
            public UISliderButton()
            {
                Padding = new UIAnchors();
                Margin = new UIAnchors();
            }

            public new UISlider Parent => (UISlider)base.Parent;

            private float _ProgressFactor;
            internal float Progress
            {
                get => _ProgressFactor;
                set
                {
                    Value = (value / ValueToProgressFactor) + MinValue;
                }
            }

            private float _Value;
            internal float Value
            {
                get => _Value;
                set
                {
                    if (value == _Value)
                        return;

                    var oldValue = _Value;
                    _Value = Math.Clamp(value, MinValue, MaxValue);
                    _ProgressFactor = Math.Clamp((_Value - MinValue) * ValueToProgressFactor, 0, 1);

                    Parent.OnSliderValueChangedInternal(new SliderValueChangedArgs
                    {
                        OldValue = oldValue,
                        NewValue = _Value,
                        Button = this,
                    });
                }
            }

            private float MinValue => Parent.MinValue;
            private float MaxValue => Parent.MaxValue;

            private float SliderThickness => Parent.SliderThickness;

            private Box2 ParentRect => Parent.AbsolutePaddingRect;
            private Vector2 SliderSize => SwapAxis(new Vector2(SliderThickness, SwapAxis(ParentRect.Size).Y));
            private float PixelRange => SwapAxis(ParentRect.Size).X - SwapAxis(SliderSize).X;
            private float PixelToProgressFactor => 1 / PixelRange;
            private float ValueToProgressFactor => 1 / (MaxValue - MinValue);

            private Orientation Orientation => Parent.Orientation;
            private Vector2 SwapAxis(Vector2 vec) => Orientation == Orientation.Horizontal ? vec : new Vector2(vec.Y, vec.X);

            internal override void SetComponentSize()
            {
                Size = SliderSize;
                var xPos = PixelRange * Progress;
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
                StartProgress = Progress;
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
                Progress = StartProgress + progressDiff;
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

            MinValue = 0;
            MaxValue = 100;
            Value = 0;
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

            Image.Mutate(ctx => ctx.Clear(bgColor));
            Image.Mutate(ctx => ctx.DrawButton(BorderSize, borderColor, BorderRadius));
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
