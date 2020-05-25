// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    /// <inheritdoc/>
    public class Tween2 : Tween<Vector2>
    {
        public static TweenFunc<Vector2> Circle()
            => (p) => new Vector2(AxMath.CosNorm(p), AxMath.SinNorm(p));
        public static TweenFunc<Vector2> Circle(float scale)
            => (p) => new Vector2(AxMath.CosNorm(p) * scale, AxMath.SinNorm(p) * scale);
    }
}
