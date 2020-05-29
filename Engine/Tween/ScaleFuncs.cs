// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public static class ScaleFuncs
    {
        public static ScaleFunc Linear()
            => (p) => p;

        public static ScaleFunc LinearReverse()
            => (p) => 1 - p;

        public static ScaleFunc Linear(float scale)
            => (p) => p * scale;

        public static ScaleFunc LinearReverse(float scale)
            => (p) => scale - (p * scale);

        /// <summary>
        /// A linear progress scale function.
        /// </summary>
        public static readonly ScaleFunc Linear_ = LinearImpl;

        /// <summary>
        /// A quadratic (x^2) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc Power2EaseIn = QuadraticEaseInImpl;

        /// <summary>
        /// A quadratic (x^2) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc Power2EaseOut = QuadraticEaseOutImpl;

        /// <summary>
        /// A quadratic (x^2) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc Power2EaseInOut = QuadraticEaseInOutImpl;

        /// <summary>
        /// A cubic (x^3) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc Power3EaseIn = CubicEaseInImpl;

        /// <summary>
        /// A cubic (x^3) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc Power3EaseOut = CubicEaseOutImpl;

        /// <summary>
        /// A cubic (x^3) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc Power3EaseInOut = CubicEaseInOutImpl;

        /// <summary>
        /// A quartic (x^4) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc Power4EaseIn = QuarticEaseInImpl;

        /// <summary>
        /// A quartic (x^4) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc Power4EaseOut = QuarticEaseOutImpl;

        /// <summary>
        /// A quartic (x^4) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc Power4EaseInOut = QuarticEaseInOutImpl;

        /// <summary>
        /// A quintic (x^5) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc Power5EaseIn = QuinticEaseInImpl;

        /// <summary>
        /// A quintic (x^5) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc Power5EaseOut = QuinticEaseOutImpl;

        /// <summary>
        /// A quintic (x^5) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc Power5EaseInOut = QuinticEaseInOutImpl;

        /// <summary>
        /// A power of 10 (x^10) progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc Power10EaseIn = QuinticEaseInImpl;

        /// <summary>
        /// A power of 10 (x^10) progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc Power10EaseOut = QuinticEaseOutImpl;

        /// <summary>
        /// A power of 10 (x^10) progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc Power10EaseInOut = QuinticEaseInOutImpl;

        /// <summary>
        /// A sinusoidal progress scale function that eases in.
        /// </summary>
        public static readonly ScaleFunc SineEaseIn = SineEaseInImpl;

        /// <summary>
        /// A sinusoidal progress scale function that eases out.
        /// </summary>
        public static readonly ScaleFunc SineEaseOut = SineEaseOutImpl;

        /// <summary>
        /// A sinusoidal progress scale function that eases in and out.
        /// </summary>
        public static readonly ScaleFunc SineEaseInOut = SineEaseInOutImpl;

        private const float Pi = MathF.PI;
        private const float HalfPi = Pi / 2f;

        private static float LinearImpl(float progress) { return progress; }
        private static float QuadraticEaseInImpl(float progress) { return EaseInPower(progress, 2); }
        private static float QuadraticEaseOutImpl(float progress) { return EaseOutPower(progress, 2); }
        private static float QuadraticEaseInOutImpl(float progress) { return EaseInOutPower(progress, 2); }
        private static float CubicEaseInImpl(float progress) { return EaseInPower(progress, 3); }
        private static float CubicEaseOutImpl(float progress) { return EaseOutPower(progress, 3); }
        private static float CubicEaseInOutImpl(float progress) { return EaseInOutPower(progress, 3); }
        private static float QuarticEaseInImpl(float progress) { return EaseInPower(progress, 4); }
        private static float QuarticEaseOutImpl(float progress) { return EaseOutPower(progress, 4); }
        private static float QuarticEaseInOutImpl(float progress) { return EaseInOutPower(progress, 4); }
        private static float QuinticEaseInImpl(float progress) { return EaseInPower(progress, 5); }
        private static float QuinticEaseOutImpl(float progress) { return EaseOutPower(progress, 5); }
        private static float QuinticEaseInOutImpl(float progress) { return EaseInOutPower(progress, 5); }
        private static float Power10EaseInImpl(float progress) { return EaseInPower(progress, 10); }
        private static float Power10EaseOutImpl(float progress) { return EaseOutPower(progress, 10); }
        private static float Power10EaseInOutImpl(float progress) { return EaseInOutPower(progress, 10); }

        private static float EaseInPower(float progress, int power)
        {
            return MathF.Pow(progress, power);
        }

        private static float EaseOutPower(float progress, int power)
        {
            int sign = power % 2 == 0 ? -1 : 1;
            return (float)(sign * (Math.Pow(progress - 1, power) + sign));
        }

        private static float EaseInOutPower(float progress, int power)
        {
            progress *= 2;
            if (progress < 1)
            {
                return MathF.Pow(progress, power) / 2f;
            }
            else
            {
                int sign = power % 2 == 0 ? -1 : 1;
                return (float)(sign / 2.0 * (MathF.Pow(progress - 2, power) + (sign * 2)));
            }
        }

        private static float SineEaseInImpl(float progress)
        {
            return MathF.Sin((progress * HalfPi) - HalfPi) + 1;
        }

        private static float SineEaseOutImpl(float progress)
        {
            return MathF.Sin(progress * HalfPi);
        }

        private static float SineEaseInOutImpl(float progress)
        {
            return (MathF.Sin((progress * Pi) - HalfPi) + 1) / 2;
        }
    }
}
