// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public class GameContext : GameObject
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
            actor.AddRef(this);
            Actors.Add(actor);
            RegisterActorName(actor);
        }

        public override void Visit<T>(Action<T> action, Func<T, bool> visitChilds = null)
        {
            foreach (var act in Actors)
                act.Visit(action, visitChilds);
        }

        public void Visit(Action<Actor> action)
        {
            foreach (var act in Actors)
                action(act);
        }

        public void RemoveActor(Actor actor)
        {
            if (actor == null)
                return;

            actor.IsAttached = false;
            Actors.Remove(actor);
            UnregisterActorName(actor);

            actor.RemoveRef(this);
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

        internal IList<GameObject> ObjectsForDeallocation = new List<GameObject>(); //TODO: Sync

        public void Sync()
        {

            while (ObjectsForDeallocation.Count > 0)
            {
                foreach (var obj in ObjectsForDeallocation.ToArray())
                {
                    if (obj.RefCount == 0)
                    {
                        obj.VisitChilds<GameObject>(obj =>
                        {
                            if (obj.RefCount == 1)
                                obj.Deallocate();
                        }, child => child.RefCount <= 1);
                    }
                }

                foreach (var obj in ObjectsForDeallocation.ToArray())
                {
                    obj.DoDeallocation();
                    ObjectsForDeallocation.Remove(obj);
                }
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
                        Log.Info("{Id} {Type} {Name}", obj.ObjectId, obj.GetType().Name, obj.Name);

            RenderContext.DumpInfo(list);
        }

        public void Init()
        {
            TimeWatcher = new Stopwatch();
            TimeWatcher.Start();
        }

        private Stopwatch TimeWatcher;
        public TimeSpan Time { get; set; }

        internal void UpdateTime()
        {
            Time = TimeWatcher.Elapsed;
        }

        public void OnScreenResize(ScreenResizeEventArgs e)
        {
            Visit<Actor>(c => c.OnScreenResize(e));
        }

        public void OnMouseButton(MouseButtonArgs e)
        {
            Visit<Actor>(c => c.OnMouseButton(e), c => !e.Handled);
        }

        public void OnMouseMove(MouseMoveArgs e)
        {
            Visit<Actor>(c => c.OnMouseMove(e), c => !e.Handled);
        }

    }

}
