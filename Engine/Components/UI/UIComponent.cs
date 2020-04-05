// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;
using OpenToolkit.Mathematics;
using SixLabors.Primitives;

namespace Aximo.Engine
{

    public class UIComponent : GraphicsScreenTextureComponent
    {

        public UIComponent() : this(new Vector2i(100, 100))
        {
        }

        public UIComponent(Vector2i size) : base(size)
        {
        }

        internal virtual void CalculateSizes()
        {
            // if (Parent == null)
            //     return;

            if (Parent is UIComponent p)
                AbsoluteOuterRect = p.AbsoluteClientRect;
            else
                AbsoluteOuterRect = new Box2(Vector2.Zero, RenderContext.Current.ScreenSize.ToVector2());

            // TODO: Shrink AbsoluteOuterRect if Dock is set.
            // ...

            var oldDrawSize = DrawSize;

            Vector2 relSize;
            Vector2 relPos;

            // Draw

            relSize = AbsoluteOuterRect.Size - Margin.Size;
            relPos = Margin.Min;

            RelatativeDrawRect = BoxHelper.FromSize(relPos, relSize);
            AbsoluteDrawRect = BoxHelper.FromSize(AbsoluteOuterRect.Min + relPos, relSize);

            // Client

            relSize = AbsoluteOuterRect.Size - Margin.Size - Border.Size;
            relPos = Margin.Min + Border.Min;

            RelatativeClientRect = BoxHelper.FromSize(relPos, relSize);
            AbsoluteClientRect = BoxHelper.FromSize(AbsoluteOuterRect.Min + relPos, relSize);

            // Padding

            relSize = AbsoluteOuterRect.Size - Margin.Size - Border.Size - PaddingInternal.Size;
            relPos = Margin.Min + Border.Min + PaddingInternal.Min;

            RelatativePaddingRect = BoxHelper.FromSize(relPos, relSize);
            AbsolutePaddingRect = BoxHelper.FromSize(AbsoluteOuterRect.Min + relPos, relSize);

            if (DrawSize != oldDrawSize)
            {
                ResizeImage(DrawSize.ToVector2i());
                RectanglePixels = AbsoluteClientRect.ToRectangleF();
            }
        }

        public override void UpdateFrame()
        {
            CalculateSizes();
        }

        // Size+Border+Margin
        private Box2 AbsoluteOuterRect; // Absoute Rect of this control incl. Margin

        // Size+Border
        private Box2 RelatativeDrawRect;
        private Box2 AbsoluteDrawRect;

        // Size
        private Box2 RelatativeClientRect;
        private Box2 AbsoluteClientRect;

        // Size-Padding
        private Box2 RelatativePaddingRect;
        private Box2 AbsolutePaddingRect;

        private Vector2 DrawSize => RelatativeDrawRect.Size;

        protected internal virtual UIAnchors PaddingInternal => new UIAnchors();

        public UIAnchors Margin;
        public Box2 ClientRect;
        public UIAnchors Border;
        public UIDock Dock;

        private RectangleF? _RectangleUV;
        public RectangleF RectangleUV
        {
            set
            {
                if (_RectangleUV == value)
                    return;

                var pos = new Vector3(
                    ((value.X + (value.Width / 2f)) * 2) - 1.0f,
                    ((1 - (value.Y + (value.Height / 2f))) * 2) - 1.0f,
                    0);

                var scale = new Vector3(value.Width, -value.Height, 1.0f);
                RelativeTranslation = pos;
                RelativeScale = scale;
                _RectanglePixels = null;
                _RectangleUV = value;
            }
        }

        private RectangleF? _RectanglePixels;
        public RectangleF RectanglePixels
        {
            set
            {
                if (_RectanglePixels == value)
                    return;

                var pos1 = new Vector2(value.X, value.Y) * RenderContext.Current.PixelToUVFactor;
                var pos2 = new Vector2(value.Right, value.Bottom) * RenderContext.Current.PixelToUVFactor;

                RectangleUV = new RectangleF(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y);
                _RectangleUV = null;
                _RectanglePixels = value;
            }
        }
    }

}
