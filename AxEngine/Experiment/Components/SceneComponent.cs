// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using OpenTK;

namespace Aximo.Engine
{
    public class SceneComponent : ActorComponent
    {

        private List<SceneComponent> _Components;
        public ICollection<SceneComponent> Components { get; private set; }

        public SceneComponent Parent { get; private set; }

        public SceneComponent()
        {
            _Components = new List<SceneComponent>();
            Components = new ReadOnlyCollection<SceneComponent>(_Components);

            _ParentComponents = new List<SceneComponent>();
            ParentComponents = new ReadOnlyCollection<SceneComponent>(_ParentComponents);
        }

        public bool CanAttach(SceneComponent child)
        {
            return true;
        }

        public void AddComponent(SceneComponent child)
        {
            if (CanAttach(child))
                throw new InvalidOperationException("Can't attach this child");

            _Components.Add(child);
            child.Parent = this;

            child.SetParents();
        }

        private void SetParents()
        {
            _ParentComponents.Clear();
            _ParentComponents.AddRange(Parent.ParentComponents);
            _ParentComponents.Add(Parent);

            foreach (var component in Components)
                component.SetParents();
        }

        public void RemoveComponent(SceneComponent child)
        {
            if (child.Parent == null)
                return;

            if (!_Components.Remove(child))
                return;

            child.Parent = null;
        }

        private List<SceneComponent> _ParentComponents;
        public IReadOnlyList<SceneComponent> ParentComponents { get; private set; }

        public SceneComponent RootComponent => ParentComponents.FirstOrDefault();

        public Quaternion RelativeRotation { get; set; }
        public Vector3 RelativeScale { get; set; } = Vector3.One;
        public Vector3 RelativeLocation { get; set; }

        public Transform Transform => new Transform(RelativeRotation, RelativeScale, RelativeLocation);

    }

}
