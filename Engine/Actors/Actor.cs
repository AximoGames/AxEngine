// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

#pragma warning disable CA1721 // Property names should not match get methods

namespace Aximo.Engine
{

    public class Actor : GameObject
    {
        public bool IsAttached { get; internal set; }

        public string Name { get; set; }

        private IList<ActorComponent> _Components;
        public IList<ActorComponent> Components { get; private set; }

        public SceneComponent RootComponent { get; private set; }

        public Actor()
        {
            _Components = new List<ActorComponent>(); //TODO: Sync
            Components = new ReadOnlyCollection<ActorComponent>(_Components);
        }

        public Actor(ActorComponent component) : this()
        {
            AddComponent(component);
        }

        public override void Visit<T>(Action<T> action, Func<T, bool> visitChilds = null)
        {
            base.Visit(action, visitChilds);

            if (visitChilds != null)
                if (this is T obj)
                    if (!visitChilds.Invoke(obj))
                        return;

            foreach (var comp in Components)
                comp.Visit(action, visitChilds);
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

        internal virtual void SyncChanges()
        {
            foreach (var comp in Components.ToArray())
                comp.SyncChanges();
        }

        public void AddComponent(ActorComponent component)
        {
            component.SetActor(this);
            _Components.Add(component);
            component.AddRef(this);

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

            component.RemoveRef(this);
            UnregisterComponentName(component);
        }

        public virtual void OnScreenResize(ScreenResizeEventArgs e)
        {
            RootComponent?.Visit<SceneComponent>(obj => obj.OnScreenResize(e));
        }

        public virtual void OnMouseButton(MouseButtonArgs e)
        {
            RootComponent?.Visit<SceneComponent>(obj => obj.OnMouseButton(e));
        }

        public virtual void OnMouseMove(MouseMoveArgs e)
        {
            RootComponent?.Visit<SceneComponent>(obj => obj.OnMouseMove(e));
        }

    }

}
