// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
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

        public UIComponent() : this(new Vector2(100, 100))
        {
        }

        public UIComponent(Vector2 size) : base(size)
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

            if (!AbsoluteDrawRect.Size.Approximately(oldDrawRect.Size))
            {
                ResizeImage(DrawSize);
            }
            if (AbsoluteDrawRect != oldDrawRect)
            {
                SetRectangleScaled(AbsoluteDrawRect.ToRectangleF());
                if (!AbsoluteDrawRect.Size.Approximately(oldDrawRect.Size))
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

                var newAbsoluteOuterRect = BoxHelper.FromSize(Location, size);
                //if (newAbsoluteOuterRect == AbsoluteOuterRect) // TODO: Fix
                //    return;

                AbsoluteOuterRect = newAbsoluteOuterRect;

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

        public Vector2 AbsoluteCenter => AbsoluteClientRect.Center;

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

        public Vector2 OuterSize
        {
            get => Size + Border.Size + Margin.Size;
            set
            {
                Size = value - Border.Size - Margin.Size;
            }
        }

        public UIAnchors Border;
        public UIDock Dock;

        private RectangleF? _RectangleUV;
        public void SetRectangleUV(RectangleF value)
        {
            if (_RectangleUV == value)
                return;

            Transform = TransformUtil.TransformUVRectangleToScreenSpace(value);
            _RectanglePixels = null;
            _RectangleUV = value;
        }

        private RectangleF? _RectanglePixels;
        public void SetRectanglePixels(RectangleF value)
        {
            if (_RectanglePixels == value)
                return;

            Transform = TransformUtil.TransformPixelRectangleToScreenSpace(value);
            _RectangleUV = null;
            _RectanglePixels = value;
        }

        private RectangleF? _RectangleScaled;
        public void SetRectangleScaled(RectangleF value)
        {
            if (_RectangleScaled == value)
                return;

            Transform = TransformUtil.TransformScaleRectangleToScreenSpace(value);
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
