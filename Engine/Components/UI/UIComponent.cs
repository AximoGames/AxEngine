﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Windows;
using Aximo.Render;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;

namespace Aximo.Engine.Components.UI
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
                SetRectangleScaled(AbsoluteDrawRect.ToRectangleF());
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
                    size.X = SceneContext.Current.ScreenScaledSize.X;
                if (size.Y == 0)
                    size.Y = SceneContext.Current.ScreenScaledSize.Y;
                AbsoluteOuterRect = BoxHelper.FromSize(Location, size);

                CalculateSizes();
            }

            Material.UseTransparency = true; // TODO: Set only where required
        }

        public override bool ContainsScreenCoordinate(Vector2 pos)
        {
            return AbsoluteDrawRect.Contains(pos);
        }

        // Size+Border+Margin
        internal Box2 AbsoluteOuterRect; // Absoute Rect of this control incl. Margin

        // Size+Border
        internal Box2 RelatativeDrawRect;
        internal Box2 AbsoluteDrawRect;

        // Size
        internal Box2 RelatativeClientRect;
        internal Box2 AbsoluteClientRect;

        // Size-Padding
        internal Box2 RelatativePaddingRect;
        internal Box2 AbsolutePaddingRect;

        private Vector2 DrawSize => RelatativeDrawRect.Size;

        internal virtual UIAnchors PaddingInternal => new UIAnchors();

        public UIAnchors Margin;

        public Vector2 Location;
        // Inner Padding Size!
        private Vector2 _Size;
        public Vector2 Size
        {
            get => _Size;
            set
            {
                if (_Size == value)
                    return;
                _Size = value;
                PropertyChanged();
            }
        }

        public UIAnchors Border;
        public UIDock Dock;

        private RectangleF? _RectangleUV;
        public void SetRectangleUV(RectangleF value)
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

        private RectangleF? _RectanglePixels;
        public void SetRectanglePixels(RectangleF value)
        {
            if (_RectanglePixels == value)
                return;

            var pos1 = new Vector2(value.X, value.Y) * RenderContext.Current.PixelToUVFactor;
            var pos2 = new Vector2(value.Right, value.Bottom) * RenderContext.Current.PixelToUVFactor;

            SetRectangleUV(new RectangleF(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y));
            _RectangleUV = null;
            _RectanglePixels = value;
        }

        private RectangleF? _RectangleScaled;
        public void SetRectangleScaled(RectangleF value)
        {
            if (_RectangleScaled == value)
                return;

            var pos1 = new Vector2(value.X, value.Y) * SceneContext.Current.ScaleToPixelFactor;
            var pos2 = new Vector2(value.Right, value.Bottom) * SceneContext.Current.ScaleToPixelFactor;

            SetRectanglePixels(new RectangleF(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y));
            _RectangleScaled = value;
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
    }
}
