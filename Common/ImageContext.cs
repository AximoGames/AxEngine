// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using OpenToolkit.Mathematics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Aximo
{

    public class ImageContext
    {
        public Image Image { get; private set; }
        private Vector2 TransformFactor;

        public ImageContext(Image image) : this(image, Vector2.One)
        {
        }

        public ImageContext(Image image, Vector2 transformFactor)
        {
            TransformFactor = transformFactor;
            SetImage(image);
            UpdateFont();
        }

        public void SetImage(Image image)
        {
            Image = image;
        }

        internal PointF Transform(Vector2 location)
        {
            return new PointF(location.X * TransformFactor.X, location.Y * TransformFactor.Y);
        }

        internal float Transform(float size)
        {
            return size * TransformFactor.X;
        }

        private void UpdateFont()
        {
            Font = new Font(FontFamily, Transform(FontSize), FontStyle);
        }

        private FontStyle _FontStyle = FontStyle.Regular;
        public FontStyle FontStyle
        {
            get
            {
                return _FontStyle;
            }
            set
            {
                if (value == _FontStyle)
                    return;

                _FontStyle = value;
                UpdateFont();
            }
        }

        private float _FontSize = 15;
        public float FontSize
        {
            get
            {
                return _FontSize;
            }
            set
            {
                if (value == _FontSize)
                    return;

                _FontSize = value;
                UpdateFont();
            }
        }

        private FontFamily FontFamily = SharedLib.DefaultFontFamily();
        private Font Font;
        private IBrush Brush;
        public float StrokeThickness = 1;

        public VerticalAlignment VerticalTextAlignment { get; set; } = VerticalAlignment.Top;

        public void FillStyle(Color color)
        {
            Brush = new SolidBrush(color);
        }

        public void DrawText(string text, Vector2 location)
        {
            var options = new TextGraphicsOptions
            {
                TextOptions = new TextOptions
                {
                    VerticalAlignment = VerticalTextAlignment,
                },
            };

            Image.Mutate(ctx => ctx.DrawText(options, text, Font, Brush, Transform(location)));
        }

        public void DrawText(string text, Color color, Vector2 location)
        {
            var options = new TextGraphicsOptions
            {
                TextOptions = new TextOptions
                {
                    VerticalAlignment = VerticalTextAlignment,
                },
            };

            Image.Mutate(ctx => ctx.DrawText(options, text, Font, color, Transform(location)));
        }

        public void Clear(Color color)
        {
            Image.Mutate(ctx => ctx.Clear(color));
        }

        public void BeginPath()
        {
            Paths.Clear();
        }

        public void MoveTo(PointF to)
        {
            CurrentPathPosition = to;
        }

        public void MoveTo(Vector2 to)
        {
            MoveTo(new PointF(to.X, to.Y));
        }

        private List<IPath> Paths = new List<IPath>();
        private PointF CurrentPathPosition = PointF.Empty;

        public void LineTo(PointF to)
        {
            Paths.Add(new Polygon(new LinearLineSegment(CurrentPathPosition, to)));
            CurrentPathPosition = to;
        }

        public void LineTo(Vector2 to)
        {
            LineTo(new PointF(to.X, to.Y));
        }

        public void Stroke()
        {
            var pathCollection = new PathCollection(Paths);
            Image.Mutate(ctx => ctx.Draw(Brush, StrokeThickness, pathCollection));
        }

        public event Action OnFlush;
        public void Flush()
        {
            OnFlush?.Invoke();
        }
    }

    public static class ImageContextExtensions
    {
        public static void DrawButton(this ImageContext ctx, float borderTickness, Color borderColor, float cornerRadius = 0)
        {
            ctx.Image.Mutate(c => c.DrawButton(ctx.Transform(borderTickness), borderColor, ctx.Transform(cornerRadius)));
        }

        public static void DrawButton(this ImageContext ctx, UIAnchors borderTickness, Color borderColor, float cornerRadius = 0)
        {
            ctx.Image.Mutate(c => c.DrawButton(ctx.Transform(borderTickness.Left), borderColor, ctx.Transform(cornerRadius)));
        }
    }
}
