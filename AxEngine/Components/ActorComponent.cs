// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Aximo.Engine
{

    public class ActorComponent : EngineObject
    {
        public int ComponentId { get; private set; }

        public string Name { get; set; }

        public bool HasChanges { get; internal set; } = true;

        private static int LastComponentId;

        private static int GetNewComponentId()
        {
            return Interlocked.Increment(ref LastComponentId);
        }

        public ActorComponent()
        {
            ComponentId = GetNewComponentId();
        }

        public virtual Actor Actor { get; private set; }

        internal void SetActor(Actor actor)
        {
            if (Actor == actor)
                return;

            if (Actor != null)
                throw new InvalidOperationException("Actor already attached");

            Actor = actor;
        }

        public virtual void Visit(Action<SceneComponent> action)
        {
        }

        public virtual void Detach()
        {
            Actor?.RemoveComponent(this);
            Actor = null;

            Deallocate();
        }

        internal virtual void UpdateFrameInternal()
        {
            UpdateFrame();
        }

        public virtual void UpdateFrame()
        {
        }

        internal virtual void PropagateChangesUp()
        {
        }

        internal virtual void PropagateChangesDown()
        {
        }

        internal virtual void SyncChanges()
        {
            HasChanges = false;
        }

        protected internal void Update()
        {
            HasChanges = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Detach();
            }
            base.Dispose(disposing);
        }

    }
}
