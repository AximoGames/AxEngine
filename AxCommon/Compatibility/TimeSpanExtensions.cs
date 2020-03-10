// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System
{
    public static class TimeSpanExtensions
    {

        public static TimeSpan Multiply(this TimeSpan ts, double value)
        {
            // TODO: dotnet core has a build in Operator.
            return new TimeSpan((long)(ts.Ticks * value));
        }

    }

}
