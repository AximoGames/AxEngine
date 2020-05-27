// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    /// <inheritdoc/>
    public class Tween2 : Tween<Vector2>
    {

        public Tween2()
        {
            LerpFunc = Vector2.Lerp;
        }

        public static LerpFunc<Vector2> Circle()
            => (start, end, p) => new Vector2(AxMath.CosNorm(p), AxMath.SinNorm(p));

        public static LerpFunc<Vector2> Circle(float scale)
            => (start, end, p) => new Vector2(AxMath.CosNorm(p) * scale, AxMath.SinNorm(p) * scale);
    }
}
