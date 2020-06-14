// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Aximo.Engine
{
    public abstract class SceneObject : IDisposable
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<SceneObject>();
        private static int LastGameObjectId;

        private int _ObjectId;
        public int ObjectId => _ObjectId;

        public string Name { get; set; }

        public SceneObject()
        {
            _ObjectId = GetNewGameObjectId();
        }

        private static int GetNewGameObjectId()
        {
            return Interlocked.Increment(ref LastGameObjectId);
        }

        internal int RefCount => Consumers.Count;

        private List<SceneObject> Consumers = new List<SceneObject>();

        internal void AddRef(SceneObject consumer)
        {
            lock (Consumers)
            {
                if (!Consumers.Contains(consumer))
                {
                    Consumers.Add(consumer);
                }
            }

            DeallocateUndo();
        }

        internal void RemoveRef(SceneObject consumer)
        {
            lock (Consumers)
            {
                if (Consumers.Contains(consumer))
                {
                    Consumers.Remove(consumer);
                }
            }

            if (RefCount == 0)
                Deallocate();
        }

        internal virtual void DeallocateUndo()
        {
            if (!HasDeallocation)
                return;

            HasDeallocation = false;
            lock (SceneContext.Current.ObjectsForDeallocation)
                SceneContext.Current.ObjectsForDeallocation.Remove(this);
        }

        internal virtual void Deallocate()
        {
            if (HasDeallocation)
                return;

            HasDeallocation = true;
            lock (SceneContext.Current.ObjectsForDeallocation)
                if (!SceneContext.Current.ObjectsForDeallocation.Contains(this))
                    SceneContext.Current.ObjectsForDeallocation.Add(this);
        }

        private protected bool HasDeallocation;

        // Only this object, no childs
        internal virtual void DoDeallocation()
        {
            HasDeallocation = false;
        }

        internal bool Disposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (SceneContext.IsUpdateThread)
            {
                Deallocate();
                return;
            }

            if (disposing)
            {
                DoDeallocation();
                Consumers?.Clear();
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            Disposed = true;
        }

        ~SceneObject()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        public virtual void Visit<T>(Action<T> action, Func<T, bool> visitChilds = null)
            where T : SceneObject
        {
            if (this is T)
                action((T)this);
        }

        public virtual IEnumerable<T> Find<T>(Func<T, bool> visitChilds = null)
            where T : SceneObject
        {
            if (this is T)
                yield return (T)this;
        }

        public void VisitChilds<T>(Action<T> action, Func<T, bool> visitChilds = null)
            where T : SceneObject
        {
            Visit(obj =>
            {
                if (obj != this)
                    action(obj);
            }, visitChilds);
        }

        internal virtual void DumpInfo(bool list)
        {
            Log.ForContext("DumpInfo").Info("{Type} #{Id} {Name}", GetType().Name, ObjectId, Name);
            if (list)
                VisitChilds<SceneObject>(a => a.DumpInfo(false));
        }
    }
}
