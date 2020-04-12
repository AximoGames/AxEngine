// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Aximo.Engine
{
    public static class AnimationFuncs
    {
        public static AnimationFunc Linear()
        {
            return (p) => { return p; };
        }

        public static AnimationFunc LinearReverse()
        {
            return (p) => { return 1 - p; };
        }

        public static AnimationFunc Linear(float scale)
        {
            return (p) => { return p * scale; };
        }

        public static AnimationFunc LinearReverse(float scale)
        {
            return (p) => { return scale - (p * scale); };
        }
    }
}