using System;
using System.Linq;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public static class PathBuilder
    {
        public static Vector2[] Quad()
        {
            return new Vector2[]
            {
                new Vector2(-0.5f, -0.5f),
                new Vector2(0.5f, -0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-0.5f, 0.5f),
            };
        }

        public static Vector2[] Circle(int slices = 8)
        {
            var points = new Vector2[slices];
            for (var i = 0; i < slices; i++)
            {
                var angle = ((MathF.PI * 2) / slices) * i;
                points[i].X = MathF.Cos(angle) * 0.5f;
                points[i].Y = MathF.Sin(angle) * 0.5f;
            }
            return points;
        }

        public static Vector2[] ClosePath(this Vector2[] points)
        {
            if (points[0].Approximately(points[points.Length - 1]))
                return points;

            return points.Append(points[0]).ToArray();
        }

        public static Vector3[] ToVector3(this Vector2[] points)
        {
            return points.Select(p => new Vector3(p)).ToArray();
        }

        public static bool Approximately(this in Vector2 value, in Vector2 other)
        {
            return AxMath.Approximately(value.X, other.X) && AxMath.Approximately(value.Y, other.Y);
        }

    }
}