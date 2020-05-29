// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    /// <inheritdoc/>
    public class Tween3 : Tween<Vector3>
    {
        protected override LerpFunc<Vector3> GetDefaultLerpFunc()
        {
            return Vector3.Lerp;
        }
    }
}
