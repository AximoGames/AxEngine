using System;
using OpenTK;

namespace AxEngine
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