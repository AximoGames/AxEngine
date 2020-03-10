// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Aximo
{
    public static class MathF
    {

        public const float PI = (float)Math.PI;

        public static float Round(float d)
        {
            return (float)Math.Round(d);
        }

        public static float Round(float value, int digits)
        {
            return (float)Math.Round(value, digits);
        }

        public static float Floor(float d)
        {
            return (float)Math.Floor(d);
        }

    }
}