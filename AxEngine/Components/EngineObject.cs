// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Aximo.Engine
{
    public abstract class EngineObject : IDisposable
    {

        private static int LastGameObjectId;

        private int _ObjectId;
        public int ObjectId => _ObjectId;

        public EngineObject()
        {
            _ObjectId = GetNewGameObjectId();
        }

        private static int GetNewGameObjectId()
        {
            return Interlocked.Increment(ref LastGameObjectId);
        }

        internal int RefCount => Consumers.Count;

        private List<EngineObject> Consumers = new List<EngineObject>();

        internal void AddRef(EngineObject consumer)
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

        internal void RemoveRef(EngineObject consumer)
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

        protected bool HasDeallocation;

        // Only this object, no childs
        internal virtual void DoDeallocation()
        {
            HasDeallocation = false;
        }

        internal bool Disposed { get; private set; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DoDeallocation();
                Consumers.Clear();
                // TODO: dispose managed state (managed objects)
            }
            Consumers = null;

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            Disposed = true;
        }

        ~EngineObject()
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

        public virtual void Visit(Action<EngineObject> action)
        {
            action(this);
        }

        public void VisitChilds(Action<EngineObject> action)
        {
            Visit(obj =>
            {
                if (obj != this)
                    action(obj);
            });
        }

    }
}
