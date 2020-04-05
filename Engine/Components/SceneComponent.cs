// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public class SceneComponent : ActorComponent
    {

        private IList<SceneComponent> _Components;
        public ICollection<SceneComponent> Components { get; private set; }

        public SceneComponent Parent { get; private set; }

        private bool _Visible = true;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (_Visible == value)
                    return;
                Update();
                _Visible = value;
            }
        }

        public SceneComponent()
        {
            _Components = new List<SceneComponent>(); //TODO: Sync
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
            child.AddRef(child);
            child.Parent = this;

            child.SetParents();

            Actor?.RegisterComponentName(child);

            child.UpdateParent();
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

            child.RemoveRef(this);

            child.Parent = null;

            Actor?.RegisterComponentName(child);
        }

        private List<SceneComponent> _ParentComponents;
        public IReadOnlyList<SceneComponent> ParentComponents { get; private set; }

        public SceneComponent RootComponent => ParentComponents.Count == 0 ? null : ParentComponents[0];

        public override Actor Actor => RootComponent?.Actor ?? base.Actor;

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

        public override void Detach()
        {
            if (Parent != null)
                Parent.RemoveComponent(this);

            foreach (var child in Components)
                child.Deallocate();

            base.Detach();
        }

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

        public Transform Transform
        {
            get
            {
                return new Transform(RelativeScale, RelativeRotation, RelativeTranslation);
            }
            set
            {
                RelativeScale = value.Scale;
                RelativeRotation = value.Rotation;
                RelativeTranslation = value.Translation;
            }
        }

        //public Transform TranslationTransform { get; set; } = Transform.Identity;

        protected virtual Transform CalculateTransform()
        {
            var trans = Transform;
            //trans.Translation.Y = -trans.Translation.Y;
            //trans.Translation *= LocalTransform.Translation;

            //TransformMatrix = Matrix4.CreateScale(1, -1, 1);

            //trans.Translation *= MultiplyTransform.Translation;

            // var t = TransformMatrix.Inverted();

            // var a = t.ExtractTranslation();
            // var b = t.ExtractScale();

            // trans.Translation += TranslationTransform.Translation;
            // trans.Translation *= TranslationTransform.Scale;

            trans.Translation = (TranslationMatrix * new Vector4(trans.Translation, 1.0f)).Xyz;

            return trans;
        }

        public Matrix4 TranslationMatrix = Matrix4.Identity;

        public Matrix4 LocalToWorld()
        {
            //var trans = Matrix4.Identity;
            var trans = CalculateTransform().GetMatrix();

            //trans *= TransformMatrix.Inverted();

            var parent = Parent;
            while (parent != null)
            {
                trans *= parent.CalculateTransform().GetMatrix();
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

        protected internal bool ParentChanged;
        protected internal void UpdateParent()
        {
            Update();
            ParentChanged = true;
        }

        internal override void PropagateChangesUp()
        {
            if (TransformChanged)
                foreach (var child in Components)
                    child.UpdateTransform();

            foreach (var child in Components)
                child.PropagateChangesUp();

            base.PropagateChangesUp();
        }

        internal override void PropagateChangesDown()
        {
            foreach (var child in Components)
                child.PropagateChangesDown();

            if (HasChanges && Parent != null)
                Parent.HasChanges = true;

            base.PropagateChangesDown();
        }

        internal override void SyncChanges()
        {
            if (!HasChanges)
                return;

            ParentChanged = false;

            foreach (var comp in Components.ToArray())
                comp.SyncChanges();

            base.SyncChanges();
        }

        public virtual void OnScreenResize(ScreenResizeEventArgs e)
        {
        }

    }

}
