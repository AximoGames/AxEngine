// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<string, List<Actor>> ActorNamehash = new ConcurrentDictionary<string, List<Actor>>();

        public Actor GetActor(string name)
        {
            List<Actor> actors;
            if (!ActorNamehash.TryGetValue(name, out actors))
                return null;
            if (actors.Count == 0)
                return null;
            return actors[0];
        }

        public Actor[] GetActors(string name)
        {
            List<Actor> actors;
            if (!ActorNamehash.TryGetValue(name, out actors))
                return Array.Empty<Actor>();
            return actors.ToArray();
        }

        public void AddActor(Actor actor)
        {
            Actors.Add(actor);
            RegisterActorName(actor);
        }

        internal void RegisterActorName(Actor actor)
        {
            if (string.IsNullOrEmpty(actor.Name) && actor.Components.Count > 0)
                actor.Name = actor.Components[0].Name;

            if (string.IsNullOrEmpty(actor.Name))
                return;

            List<Actor> array;
            if (!ActorNamehash.TryGetValue(actor.Name, out array))
                ActorNamehash.TryAdd(actor.Name, array = new List<Actor>());
            array.Add(actor);
        }

        internal void UnregisterActorName(Actor actor)
        {
            if (string.IsNullOrEmpty(actor.Name))
                return;

            List<Actor> array;
            if (!ActorNamehash.TryGetValue(actor.Name, out array))
                return;
            array.Remove(actor);
        }

        // private void AddLight(LightComponent comp)
        // {
        //     LightComponents.Add(comp);
        // }

        // private void AddPrimitive(PrimitiveComponent comp)
        // {
        //     PrimitiveComponents.Add(comp);
        // }

        // private List<LightComponent> LightComponents = new List<LightComponent>();
        // private List<PrimitiveComponent> PrimitiveComponents = new List<PrimitiveComponent>();

        public void OnUpdateFrame()
        {
            foreach (var act in Actors)
                act.UpdateFrameInternal();

            foreach (var act in Actors)
                act.PropagateChangesUp();

            foreach (var act in Actors)
                act.PropagateChangesDown();
        }

        public void Sync()
        {
            foreach (var act in Actors)
                act.SyncChanges();
        }

    }

}
