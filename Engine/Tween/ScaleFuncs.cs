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
    }
}
