// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Aximo.Render;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;

namespace Aximo.Engine
{

    public abstract class UIComponent : GraphicsScreenTextureComponent
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<UIComponent>();

        public UIComponent() : this(new Vector2i(100, 100))
        {
        }

        public UIComponent(Vector2i size) : base(size)
        {
            IsAbsoluteScale = true;
            IsAbsoluteRotation = true;
            IsAbsoluteTranslation = true;
        }

        internal virtual void SetComponentSize()
        {
            // TODO: Shrink AbsoluteOuterRect if Dock is set.
            // ...

            var oldDrawRect = AbsoluteDrawRect;

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

            if (AbsoluteDrawRect.Size != oldDrawRect.Size)
            {
                ResizeImage(DrawSize.ToVector2i());
            }
            if (AbsoluteDrawRect != oldDrawRect)
            {
                RectanglePixels = AbsoluteDrawRect.ToRectangleF();
                if (AbsoluteClientRect.Size != oldDrawRect.Size)
                    OnResized();
            }
        }

        protected IEnumerable<UIComponent> UIComponents
        {
            get
            {
                foreach (var comp in Components)
                {
                    if (comp is UIComponent c)
                        yield return c;
                }
            }
        }

        // Overwrite this method for container layouts / flow control
        internal virtual void SetChildBounds()
        {
        }

        internal void CalculateSizes()
        {
            SetComponentSize();
            SetChildBounds();
            foreach (var child in UIComponents)
            {
                child.CalculateSizes();
            }
        }

        public override void UpdateFrame()
        {
            if (Parent == null || !(Parent is UIComponent))
            {
                var size = Size;
                if (size.X == 0)
                    size.X = RenderContext.Current.ScreenSize.X;
                if (size.Y == 0)
                    size.Y = RenderContext.Current.ScreenSize.Y;
                AbsoluteOuterRect = BoxHelper.FromSize(Location, size);

                CalculateSizes();
            }
        }

        // Size+Border+Margin
        protected internal Box2 AbsoluteOuterRect; // Absoute Rect of this control incl. Margin

        // Size+Border
        protected internal Box2 RelatativeDrawRect;
        protected internal Box2 AbsoluteDrawRect;

        // Size
        protected internal Box2 RelatativeClientRect;
        protected internal Box2 AbsoluteClientRect;

        // Size-Padding
        protected internal Box2 RelatativePaddingRect;
        protected internal Box2 AbsolutePaddingRect;

        private Vector2 DrawSize => RelatativeDrawRect.Size;

        protected internal virtual UIAnchors PaddingInternal => new UIAnchors();

        public UIAnchors Margin;

        public Vector2 Location;
        // Inner Padding Size!
        public Vector2 Size;

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

        protected bool _NeedRedraw = false;
        protected void Redraw()
        {
            _NeedRedraw = true;
        }

        internal override void PostUpdate()
        {
            if (!_NeedRedraw)
                return;

            _NeedRedraw = false;
            InvokeDrawControl();
        }

        protected virtual void DrawControl()
        {

        }

        protected void InvokeDrawControl()
        {
            DrawControl();
            UpdateTexture();
        }

        protected virtual void OnResized()
        {
            Redraw();
        }

        public virtual void OnMouseEnter(MouseMoveArgs e)
        {

        }

        public virtual void OnMouseLeave(MouseMoveArgs e)
        {

        }

        public bool MouseEntered { get; private set; }

        public override void OnScreenMouseMove(MouseMoveArgs e)
        {
            if (AbsoluteDrawRect.Contains(e.Position))
            {
                if (!MouseEntered)
                {
                    MouseEntered = true;
                    Log.Verbose("MouseEnter #{ObjectId} {Name}", ObjectId, Name);
                    OnMouseEnter(e);
                }

                OnMouseMove(e);
            }
            else
            {
                if (MouseEntered)
                {
                    MouseEntered = false;
                    Log.Verbose("MouseLeave #{ObjectId} {Name}", ObjectId, Name);
                    OnMouseLeave(e);
                }
            }
        }

        public override void OnScreenMouseDown(MouseButtonArgs e)
        {
            if (MouseEntered && AbsoluteDrawRect.Contains(e.Position))
                OnMouseDown(e);
        }

        public override void OnScreenMouseUp(MouseButtonArgs e)
        {
            if (MouseEntered && AbsoluteDrawRect.Contains(e.Position))
                OnMouseUp(e);
        }

    }

}
