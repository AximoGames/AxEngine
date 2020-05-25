// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    /// <inheritdoc/>
    public class Animation<TValue> : Animation
    {
        public AnimationFunc<TValue> AnimationFunc;

        public TValue Value
        {
            get
            {
                if (AnimationFunc == null)
                    return default;

                return AnimationFunc(Position);
            }
        }
    }
}
