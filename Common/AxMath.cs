// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public static class AxMath
    {
        /// <summary
        /// Compares two floating point values if they are similar.
        /// <summary>
        public static bool Approximately(float a, float b)
        {
            return Math.Abs(b - a) < Math.Max(0.000001f * Math.Max(Math.Abs(a), Math.Abs(b)), float.Epsilon * 8);
        }

        public static Vector3 Round(this Vector3 vec)
        {
            return new Vector3(MathF.Round(vec.X), MathF.Round(vec.Y), MathF.Round(vec.Z));
        }

        public static Vector3 Round(this Vector3 vec, int digits)
        {
            return new Vector3(MathF.Round(vec.X, digits), MathF.Round(vec.Y, digits), MathF.Round(vec.Z, digits));
        }

        public static Vector2i ToVector2i(this Vector2 vec)
        {
            return new Vector2i((int)vec.X, (int)vec.Y);
        }

        public static float NormalizedToRadians(float normalizedAngle)
        {
            return normalizedAngle * MathF.PI * 2;
        }

        public static Quaternion QuaternionFromNormalizedAngles(float rotationX, float rotationY, float rotationZ)
        {
            return new Quaternion(NormalizedToRadians(rotationX), NormalizedToRadians(rotationY), NormalizedToRadians(rotationZ));
        }

        public static Quaternion QuaternionFromNormalizedAngles(Vector3 normalizedAngle)
        {
            return QuaternionFromNormalizedAngles(normalizedAngle.X, normalizedAngle.Y, normalizedAngle.Z);
        }

        public static Quaternion ToQuaternion(this Vector3 normalizedAngle)
        {
            return QuaternionFromNormalizedAngles(normalizedAngle);
        }

        public static float Map(float input, float fromMin, float fromMax, float toMin, float toMax)
        {
            return ((input - fromMin) / (fromMax - fromMin) * (toMax - toMin)) + toMin;
        }

        public static Vector2 Map(Vector2 input, Vector2 fromMin, Vector2 fromMax, Vector2 toMin, Vector2 toMax)
        {
            return new Vector2(
                Map(input.X, fromMin.X, fromMax.X, toMin.X, toMax.X),
                Map(input.Y, fromMin.Y, fromMax.Y, toMin.Y, toMax.Y));
        }

        public static float MapToNDC(float input, float fromMin, float fromMax)
        {
            return Map(input, fromMin, fromMax, -1, 1);
        }

        public static float MapFromNDC(float input, float fromMin, float fromMax, float toMin, float toMax)
        {
            return Map(input, -1, 1, toMin, toMax);
        }

        public static Vector2 MapToNDC(Vector2 input, Vector2 fromMax)
        {
            return Map(input, Vector2.Zero, fromMax, new Vector2(-1), new Vector2(1));
        }

        public static Vector2 MapFromNDC(Vector2 input, Vector2 toMax)
        {
            return Map(input, new Vector2(-1), new Vector2(1), Vector2.Zero, toMax);
        }

        public static Vector2 MapFromNDC(Vector2 input, Vector2i toMax)
        {
            return Map(input, new Vector2(-1), new Vector2(1), Vector2.Zero, toMax.ToVector2());
        }
    }
}
