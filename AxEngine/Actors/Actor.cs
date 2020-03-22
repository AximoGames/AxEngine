// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Aximo.Engine
{

    public class Actor
    {
        public int ActorId { get; private set; }

        public string Name { get; set; }

        private IList<ActorComponent> _Components;
        public IList<ActorComponent> Components { get; private set; }

        public SceneComponent RootComponent { get; private set; }

        private static int LastGameObjectId;

        private static int GetNewGameObjectId()
        {
            return Interlocked.Increment(ref LastGameObjectId);
        }

        public Actor()
        {
            ActorId = GetNewGameObjectId();
            _Components = new SynchronizedCollection<ActorComponent>();
            Components = new ReadOnlyCollection<ActorComponent>(_Components);
        }

        public Actor(ActorComponent component) : this()
        {
            AddComponent(component);
        }

        private ConcurrentDictionary<string, List<ActorComponent>> ComponentNameHash = new ConcurrentDictionary<string, List<ActorComponent>>();

        public T GetComponent<T>(string name)
            where T : ActorComponent
        {
            List<ActorComponent> components;
            if (!ComponentNameHash.TryGetValue(name, out components))
                return null;
            if (components.Count == 0)
                return null;
            foreach (var comp in components)
                if (comp is T)
                    return (T)comp;

            return null;
        }

        public T GetComponent<T>()
            where T : ActorComponent
        {

            foreach (var comp in Components)
            {
                if (comp is T)
                    return (T)comp;
            }

            if (RootComponent != null)
            {
                return (T)RootComponent.GetComponent<T>();
            }

            return null;
        }

        public T[] GetComponents<T>(string name)
        {
            List<ActorComponent> components;
            if (!ComponentNameHash.TryGetValue(name, out components))
                return Array.Empty<T>();
            return components.Where(c => c is T).Cast<T>().ToArray();
        }

        internal void RegisterComponentName(ActorComponent component, bool registerChilds = true)
        {
            if (!string.IsNullOrEmpty(component.Name))
            {
                List<ActorComponent> array;
                if (!ComponentNameHash.TryGetValue(component.Name, out array))
                    ComponentNameHash.TryAdd(component.Name, array = new List<ActorComponent>());
                array.Add(component);
            }

            if (registerChilds && component is SceneComponent sc)
                foreach (var child in sc.Components)
                    RegisterComponentName(child);
        }

        internal void UnregisterComponentName(ActorComponent component, bool unregisterChilds = true)
        {
            if (!string.IsNullOrEmpty(component.Name))
            {
                List<ActorComponent> array;
                if (!ComponentNameHash.TryGetValue(component.Name, out array))
                    return;
                array.Remove(component);
            }

            if (unregisterChilds && component is SceneComponent sc)
                foreach (var child in sc.Components)
                    UnregisterComponentName(child);
        }

        internal virtual void UpdateFrameInternal()
        {
            foreach (var comp in Components)
                comp.UpdateFrameInternal();
            UpdateFrame();
        }

        public virtual void UpdateFrame()
        {
        }

        internal virtual void PropagateChangesUp()
        {
            foreach (var comp in Components)
                comp.PropagateChangesUp();
        }

        internal virtual void PropagateChangesDown()
        {
            foreach (var comp in Components)
                comp.PropagateChangesDown();
        }

        internal void SyncChanges()
        {
            foreach (var comp in Components.ToArray())
                comp.SyncChanges();
        }

        public void AddComponent(ActorComponent component)
        {
            component.SetActor(this);
            _Components.Add(component);

            if (component is SceneComponent)
                RootComponent = (SceneComponent)component;

            RegisterComponentName(component);
        }

        public void RemoveComponent(ActorComponent component)
        {
            if (!_Components.Remove(component))
                return;

            if (RootComponent == component)
                RootComponent = null;

            UnregisterComponentName(component);
        }

    }

}
