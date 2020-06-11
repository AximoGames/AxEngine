// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aximo.Engine.Windows;
using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public class SceneContext : SceneObject
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<SceneContext>();

        public float ReferenceScreenWidth { get; private set; } = 3840f;
        public Vector2 PixelToScaleFactor { get; private set; }
        public Vector2 ScaleToPixelFactor { get; private set; }
        public Vector2 ScreenScale { get; set; } = Vector2.One;
        public Vector2i ScreenPixelSize => RenderContext.ScreenPixelSize;
        public Vector2 ScreenScaledSize => ScreenPixelSize.ToVector2() * ScreenScale;

        public static SceneContext Current { get; set; }

        public static bool IsRenderThread => WindowContext.IsRenderThread;
        public static bool IsUpdateThread => WindowContext.IsUpdateThread;

        public List<IUpdateFrame> UpdateFrameObjects = new List<IUpdateFrame>();

        public void AddUpdateFrameObject(IUpdateFrame obj)
        {
            UpdateFrameObjects.Add(obj);
        }

        public void RemoveUpdateFrameObject(IUpdateFrame obj)
        {
            UpdateFrameObjects.Add(obj);
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

            foreach (var act in Actors)
                act.PostUpdate();
        }

        internal SynchronizedCollection<SceneObject> ObjectsForDeallocation = new SynchronizedCollection<SceneObject>();

        public void Sync()
        {
            while (ObjectsForDeallocation.Count > 0)
            {
                foreach (var obj in ObjectsForDeallocation.ToArray())
                {
                    if (obj.RefCount == 0)
                    {
                        obj.VisitChilds<SceneObject>(obj =>
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

        internal override void DumpInfo(bool list)
        {
            Log.Info("Actors: {ActorCount}", Actors.Count);
            if (list)
                lock (Actors)
                    foreach (var obj in Actors)
                        obj.DumpInfo(list);

            RenderContext.DumpInfo(list);
        }

        public void Init()
        {
            SetScale();
            TimeWatcher = new Stopwatch();
            TimeWatcher.Start();
            EngineAssets.Init();
        }

        private void SetScale()
        {
            ScreenScale = Application.Current.GetScreenPixelScale();
            PixelToScaleFactor = ScreenScale;
            ScaleToPixelFactor = Vector2.Divide(Vector2.One, ScreenScale);
        }

        private Stopwatch TimeWatcher;
        public TimeSpan Time { get; set; }

        internal void UpdateTime()
        {
            Time = TimeWatcher.Elapsed;
        }

        public void OnScreenResize(ScreenResizeEventArgs e)
        {
            SetScale();
            Visit<Actor>(c => c.OnScreenResize(e));
        }

        public void OnScreenMouseUp(MouseButtonArgs e)
        {
            Visit<Actor>(c => c.OnScreenMouseUp(e), c => !e.Handled);
        }

        public void OnScreenMouseDown(MouseButtonArgs e)
        {
            Visit<Actor>(c => c.OnScreenMouseDown(e), c => !e.Handled);
        }

        public void OnScreenMouseMove(MouseMoveArgs e)
        {
            Visit<Actor>(c => c.OnScreenMouseMove(e), c => !e.Handled);
        }
    }
}
