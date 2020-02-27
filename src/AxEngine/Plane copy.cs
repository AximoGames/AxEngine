using System;
using OpenTK;

namespace AxEngine
{
    public class AxMath
    {

        /// <summary
        /// Compares two floating point values if they are similar.
        /// <summary>
        public static bool Approximately(float a, float b)
        {
            return Math.Abs(b - a) < Math.Max(0.000001f * Math.Max(Math.Abs(a), Math.Abs(b)), float.Epsilon * 8);
        }

    }
}