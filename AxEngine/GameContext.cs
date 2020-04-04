// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public class GameContext
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<GameContext>();

        public static GameContext Current { get; set; }

        public List<Animation> Animations = new List<Animation>();

        public void AddAnimation(Animation animation)
        {
            Animations.Add(animation);
        }

        public IList<Actor> Actors = new List<Actor>(); //TODO: Sync
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
            if (actor == null)
                return;

            actor.IsAttached = true;
            Actors.Add(actor);
            RegisterActorName(actor);
        }

        public virtual void Visit(Action<SceneComponent> action)
        {
            foreach (var act in Actors)
                act.Visit(action);
        }

        public void RemoveActor(Actor actor)
        {
            if (actor == null)
                return;

            foreach (var comp in actor.Components)
                comp.Deallocate();

            actor.IsAttached = false;
            Actors.Remove(actor);
            UnregisterActorName(actor);
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

        internal IList<ActorComponent> ComponentsForDeallocation = new List<ActorComponent>(); //TODO: Sync

        public void Sync()
        {
            foreach (var comp in ComponentsForDeallocation.ToArray())
            {
                comp.DoDeallocation();
                ComponentsForDeallocation.Remove(comp);
            }

            foreach (var act in Actors.ToArray())
                act.SyncChanges();

            RenderContext.DeleteOrphaned();
        }

        private RenderContext RenderContext => RenderContext.Current;

        public Vector4 BackgroundColor
        {
            get => RenderContext.BackgroundColor;
            set => RenderContext.BackgroundColor = value;
        }

        public void DumpInfo(bool list)
        {
            Log.Info("Actors: {ActorCount}", Actors.Count);
            if (list)
                lock (Actors)
                    foreach (var obj in Actors)
                        Log.Info("{Id} {Type} {Name}", obj.ActorId, obj.GetType().Name, obj.Name);

            RenderContext.DumpInfo(list);
        }

    }

}
