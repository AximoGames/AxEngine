// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public static class VectorExtensions
    {
        public static Vector2i InvertY(this Vector2i value)
        {
            return new Vector2i(value.X, -value.Y);
        }

        public static Vector2 InvertY(this Vector2 value)
        {
            return new Vector2(value.X, -value.Y);
        }

        public static IEnumerable<Line2> ToLines(this IEnumerable<Vector2> points)
        {
            var firstEntry = true;
            Vector2 a = Vector2.Zero;
            foreach (var b in points)
            {
                if (firstEntry)
                    firstEntry = false;
                else
                    yield return new Line2(a, b);

                a = b;
            }
        }

    }

    public struct Line2
    {
        public Vector2 A;
        public Vector2 B;

        public Line2(Vector2 a, Vector2 b)
        {
            A = a;
            B = b;
        }
    }

}
