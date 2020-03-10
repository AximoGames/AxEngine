// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Aximo.Engine
{

    public class Actor
    {
        public int ActorId { get; private set; }

        private List<ActorComponent> _Components;
        public ICollection<ActorComponent> Components { get; private set; }

        private static int LastGameObjectId;

        private static int GetNewGameObjectId()
        {
            return Interlocked.Increment(ref LastGameObjectId);
        }

        public Actor()
        {
            ActorId = GetNewGameObjectId();
            _Components = new List<ActorComponent>();
            Components = new ReadOnlyCollection<ActorComponent>(_Components);
        }

        public void AddComponent(ActorComponent component)
        {
            component.SetActor(this);
            _Components.Add(component);
        }

        public void RemoveComponent(ActorComponent component)
        {
            component.Detach();
            _Components.Remove(component);
        }

    }

}
