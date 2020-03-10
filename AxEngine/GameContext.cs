// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Aximo.Engine
{

    public class GameContext
    {

        public static GameContext Current { get; set; }

        public List<Animation> Animations = new List<Animation>();

        public void AddAnimation(Animation animation)
        {
            Animations.Add(animation);
        }

    }

}
