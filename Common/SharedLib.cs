// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using SixLabors.Fonts;

namespace Aximo
{
    public static class SharedLib
    {
        public static CultureInfo LocaleInvariant = CultureInfo.InvariantCulture;
        public static FontFamily DefaultFontFamily()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return SystemFonts.Find("Arial");
            else
                return SystemFonts.Find("DejaVu Sans");

        }
    }
}