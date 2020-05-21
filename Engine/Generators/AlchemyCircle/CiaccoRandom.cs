// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;

namespace Aximo.Generators.AlchemyCircle
{
    public class CiaccoRandom
    {
        private static int superSeed = 0;

        public CiaccoRandom()
        {
        }

        public void SetSeed(int seed)
        {
            // seed can only be positive and seed range is [0 -> 9999998] seed=9999999 schould give the same as seed=0
            superSeed = (Math.Abs(seed) % 9999999) + 1;
            // init for randomness fairness
            superSeed = (superSeed * 125) % 2796203;
            superSeed = (superSeed * 125) % 2796203;
            superSeed = (superSeed * 125) % 2796203;
            superSeed = (superSeed * 125) % 2796203;
            superSeed = (superSeed * 125) % 2796203;
            superSeed = (superSeed * 125) % 2796203;
            superSeed = (superSeed * 125) % 2796203;
            superSeed = (superSeed * 125) % 2796203;
            superSeed = (superSeed * 125) % 2796203;
        }

        public int GetRand(int min, int max) // both included: getRand(0,1) will return 0s and 1s
        {
            superSeed = (superSeed * 125) % 2796203;
            return (superSeed % (max - min + 1)) + min;
        }
    }
}
