// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    /// <inheritdoc/>
    public class Tween1 : Tween<float>
    {
        public static TweenFunc<float> Linear()
            => (p) => p;

        public static TweenFunc<float> LinearReverse()
            => (p) => 1 - p;

        public static TweenFunc<float> Linear(float scale)
            => (p) => p * scale;

        public static TweenFunc<float> LinearReverse(float scale)
            => (p) => scale - (p * scale);
    }
}
