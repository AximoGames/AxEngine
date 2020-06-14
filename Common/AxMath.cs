// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using OpenToolkit.Mathematics;

namespace Aximo
{
    public static class AxMath
    {
        /// <summary>
        /// Degrees-to-radians conversion constant
        /// </summary>
        public const float Deg2Rad = MathF.PI * 2F / 360F;

        /// <summary>
        /// Radians-to-degrees conversion constant
        /// </summary>
        public const float Rad2Deg = 1F / Deg2Rad;

        /// <summary>
        /// Normal-to-radians conversion constant. Normal: 0f..1f
        /// </summary>
        public const float Norm2Rad = MathF.PI * 2F;

        /// <summary>
        /// Radians-to-normal conversion constant. Normal: 0f..1f
        /// </summary>
        public const float Rad2Norm = 1F / Norm2Rad;

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

        public static Vector2 Round(this Vector2 vec)
        {
            return new Vector2(MathF.Round(vec.X), MathF.Round(vec.Y));
        }

        public static Vector2 Round(this Vector2 vec, int digits)
        {
            return new Vector2(MathF.Round(vec.X, digits), MathF.Round(vec.Y, digits));
        }

        public static Vector2i ToVector2i(this Vector2 vec)
        {
            return new Vector2i((int)vec.X, (int)vec.Y);
        }

        public static Vector3 ToVector3(this Vector2 vec)
        {
            return new Vector3(vec.X, vec.Y, 0);
        }

        public static Vector3 ToVector3(this Vector2 vec, float z)
        {
            return new Vector3(vec.X, vec.Y, z);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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

        public static float SinDeg(float deg) => MathF.Sin(deg * Deg2Rad);
        public static float CosDeg(float deg) => MathF.Cos(deg * Deg2Rad);
        public static float SinNorm(float norm) => MathF.Sin(norm * Norm2Rad);
        public static float CosNorm(float norm) => MathF.Cos(norm * Norm2Rad);

        public static float Digits(float value) => value - MathF.Truncate(value);
        public static double Digits(double value) => value - Math.Truncate(value);
    }
}
