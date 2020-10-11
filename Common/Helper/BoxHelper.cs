// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public static class BoxHelper
    {

        public static Box3 GetBoundingBox(this IEnumerable<Vector3> points)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float minZ = float.MaxValue;

            float maxX = float.MinValue;
            float maxY = float.MinValue;
            float maxZ = float.MinValue;

            foreach (var pos in points)
            {
                minX = MathF.Min(minX, pos.X);
                minY = MathF.Min(minY, pos.Y);
                minZ = MathF.Min(minZ, pos.Z);

                maxX = MathF.Max(maxX, pos.X);
                maxY = MathF.Max(maxY, pos.Y);
                maxZ = MathF.Max(maxZ, pos.Z);
            }

            return new Box3(minX, minY, minZ, maxX, maxY, maxZ);
        }

        public static Box2 GetBoundingBox(this IEnumerable<Vector2> points)
        {
            float minX = float.MaxValue;
            float minY = float.MaxValue;

            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var pos in points)
            {
                minX = MathF.Min(minX, pos.X);
                minY = MathF.Min(minY, pos.Y);

                maxX = MathF.Max(maxX, pos.X);
                maxY = MathF.Max(maxY, pos.Y);
            }

            return new Box2(minX, minY, maxX, maxY);
        }

        public static Box2 FromSize(Vector2 location, Vector2 size)
        {
            return new Box2(location, location + size);
        }

        public static Box2 FromCenteredSize(Vector2 center, Vector2 size)
        {
            return new Box2
            {
                Center = center,
                Size = size,
            };
        }
    }
}
