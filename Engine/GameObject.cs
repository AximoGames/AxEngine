// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Aximo.Engine
{
    public abstract class GameObject : IDisposable
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<GameObject>();
        private static int LastGameObjectId;

        private int _ObjectId;
        public int ObjectId => _ObjectId;

        public string Name { get; set; }

        public GameObject()
        {
            _ObjectId = GetNewGameObjectId();
        }

        private static int GetNewGameObjectId()
        {
            return Interlocked.Increment(ref LastGameObjectId);
        }

        internal int RefCount => Consumers.Count;

        private List<GameObject> Consumers = new List<GameObject>();

        internal void AddRef(GameObject consumer)
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

        internal void RemoveRef(GameObject consumer)
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
            lock (GameContext.Current.ObjectsForDeallocation)
                GameContext.Current.ObjectsForDeallocation.Remove(this);
        }

        internal virtual void Deallocate()
        {
            if (HasDeallocation)
                return;

            HasDeallocation = true;
            lock (GameContext.Current.ObjectsForDeallocation)
                if (!GameContext.Current.ObjectsForDeallocation.Contains(this))
                    GameContext.Current.ObjectsForDeallocation.Add(this);
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
            if (GameContext.IsUpdateThread)
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

        ~GameObject()
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
            where T : GameObject
        {
            if (this is T)
                action((T)this);
        }

        public void VisitChilds<T>(Action<T> action, Func<T, bool> visitChilds = null)
            where T : GameObject
        {
            Visit<T>(obj =>
            {
                if (obj != this)
                    action(obj);
            }, visitChilds);
        }

        internal virtual void DumpInfo(bool list)
        {
            Log.ForContext("DumpInfo").Info("{Type} #{Id} {Name}", GetType().Name, ObjectId, Name);
            if (list)
                VisitChilds<GameObject>(a => a.DumpInfo(false));
        }
    }
}
