// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Aximo.Render;
using OpenTK;

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

        public List<Actor> Actors = new List<Actor>();

        public void AddActor(Actor actor)
        {
            Actors.Add(actor);

            foreach (var comp in actor.Components)
            {
                if (comp is PrimitiveComponent)
                    AddPrimitive((PrimitiveComponent)comp);
                if (comp is LightComponent)
                    AddLight((LightComponent)comp);
            }
        }

        private void AddLight(LightComponent comp)
        {
            LightComponents.Add(comp);
        }

        private void AddPrimitive(PrimitiveComponent comp)
        {
            PrimitiveComponents.Add(comp);
        }

        private List<LightComponent> LightComponents = new List<LightComponent>();
        private List<PrimitiveComponent> PrimitiveComponents = new List<PrimitiveComponent>();

        public void Sync()
        {
            foreach (var light in LightComponents)
                light.ApplyChanges();

            foreach (var prim in PrimitiveComponents)
                prim.ApplyChanges();
        }

    }

}
