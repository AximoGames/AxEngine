// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;
using Aximo.Render.OpenGL;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;

#pragma warning disable CA1044 // Properties should not be write only

namespace Aximo.Engine.Components.Geometry
{

    public static class TransformUtil
    {

        public static Transform TransformScreenUnitsToScreenSpace(Vector2 screenUnits)
        {
            var scale = Vector2.Divide(Vector2.One, screenUnits);
            var value = new RectangleF(-scale.X, -scale.Y, scale.X, scale.Y);

            var pos = new Vector3(
                ((value.X + (value.Width))) - 1.0f,
                ((1 - (value.Y + (value.Height)))),
                0);

            var trans = Transform.Identity;

            trans.Scale = new Vector3(value.Width * 2, -value.Height * 2, 1.0f);
            trans.Translation = pos;

            return trans;
        }

        public static Transform TransformUVRectangleToScreenSpace(RectangleF value)
        {
            var pos = new Vector3(
                ((value.X + (value.Width / 2f)) * 2) - 1.0f,
                ((1 - (value.Y + (value.Height / 2f))) * 2) - 1.0f,
                0);

            var trans = Transform.Identity;

            trans.Scale = new Vector3(value.Width, -value.Height, 1.0f);
            trans.Translation = pos;

            return trans;
        }

        public static Transform TransformPixelRectangleToScreenSpace(RectangleF value)
        {
            var pos1 = new Vector2(value.X, value.Y) * RenderContext.Current.PixelToUVFactor;
            var pos2 = new Vector2(value.Right, value.Bottom) * RenderContext.Current.PixelToUVFactor;

            return TransformUVRectangleToScreenSpace(new RectangleF(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y));
        }

        public static Transform TransformScaleRectangleToScreenSpace(RectangleF value)
        {
            var pos1 = new Vector2(value.X, value.Y) * SceneContext.Current.ScaleToPixelFactor;
            var pos2 = new Vector2(value.Right, value.Bottom) * SceneContext.Current.ScaleToPixelFactor;

            return TransformPixelRectangleToScreenSpace(new RectangleF(pos1.X, pos1.Y, pos2.X - pos1.X, pos2.Y - pos1.Y));
        }
    }

}
