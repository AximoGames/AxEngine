// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Engine
{
    /// <inheritdoc/>
    public class Tween1 : Tween<float>
    {
        internal static float LerpFloat(float start, float end, float progress) { return start + ((end - start) * progress); }

        protected override LerpFunc<float> GetDefaultLerpFunc()
        {
            return LerpFloat;
        }
    }
}
