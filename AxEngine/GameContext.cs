﻿using System.Collections.Generic;

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