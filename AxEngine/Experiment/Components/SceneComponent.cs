// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
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
        public SceneComponent(params SceneComponent[] childs) : this()
        {
            foreach (var child in childs)
                AddComponent(child);
        }

        public T GetComponent<T>()
            where T : ActorComponent
        {
            foreach (var comp in Components)
            {
                if (comp is T)
                    return (T)(ActorComponent)comp;

                var result = comp.GetComponent<T>();
                if (result != null)
                    return result;
            }
            return null;
        }

        public bool CanAttach(SceneComponent child)
        {
            return true;
        }

        public void AddComponent(SceneComponent child)
        {
            if (!CanAttach(child))
                throw new InvalidOperationException("Can't attach this child");

            _Components.Add(child);
            child.Parent = this;

            child.SetParents();

            Actor?.RegisterComponentName(child);
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

            Actor?.RegisterComponentName(child);
        }

        private List<SceneComponent> _ParentComponents;
        public IReadOnlyList<SceneComponent> ParentComponents { get; private set; }

        public SceneComponent RootComponent => ParentComponents.FirstOrDefault();

        public override Actor Actor => RootComponent?.Actor;

        private Vector3 _RelativeScale = Vector3.One;
        public Vector3 RelativeScale
        {
            get => _RelativeScale;
            set
            {
                if (_RelativeScale == value)
                    return;
                _RelativeScale = value;
                UpdateTransform();
            }
        }

        private Quaternion _RelativeRotation = Quaternion.Identity;
        public Quaternion RelativeRotation
        {
            get
            {
                return _RelativeRotation;
            }
            set
            {
                if (_RelativeRotation == value)
                    return;
                _RelativeRotation = value;
                UpdateTransform();
            }
        }

        private Vector3 _RelativeTranslation;
        public Vector3 RelativeTranslation
        {
            get => _RelativeTranslation;
            set
            {
                if (_RelativeTranslation == value)
                    return;
                _RelativeTranslation = value;
                UpdateTransform();
            }
        }

        public Transform Transform => new Transform(RelativeScale, RelativeRotation, RelativeTranslation);

        public Matrix4 LocalToWorld()
        {
            //var trans = Matrix4.Identity;
            var trans = Transform.GetMatrix();

            var parent = Parent;
            while (parent != null)
            {
                trans *= parent.Transform.GetMatrix();
                parent = parent.Parent;
            }
            //trans *= Transform.GetMatrix();
            //return Transform.GetMatrix();
            //return Matrix4.CreateScale(2);
            return trans;
        }

        internal override void UpdateFrameInternal()
        {
            base.UpdateFrameInternal();
            foreach (var child in Components)
                child.UpdateFrameInternal();
        }

        protected internal bool TransformChanged;
        protected internal void UpdateTransform()
        {
            Update();
            TransformChanged = true;
        }

        internal override void PropagateChanges()
        {
            if (TransformChanged)
                foreach (var child in Components)
                    child.UpdateTransform();

            foreach (var child in Components)
                child.PropagateChanges();

            base.PropagateChanges();
        }

        internal override void SyncChanges()
        {
            if (!HasChanges)
                return;

            foreach (var comp in Components)
                comp.SyncChanges();

            base.SyncChanges();
        }

    }

}
