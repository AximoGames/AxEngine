// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo
{
    public static class StringExtensions
    {
        public static bool IsUnset(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsSet(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}
