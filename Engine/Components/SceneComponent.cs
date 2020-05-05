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
        private static Serilog.ILogger Log = Aximo.Log.ForContext<GameObject>();

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

            child.OnAttached();
        }

        protected virtual void OnAttached()
        {
        }

        protected virtual void OnDetached()
        {
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

        private bool _IsAbsoluteScale;
        public bool IsAbsoluteScale
        {
            get => _IsAbsoluteScale;
            set
            {
                if (_IsAbsoluteScale == value)
                    return;
                _IsAbsoluteScale = value;
                UpdateTransform();
            }
        }

        private bool _IsAbsoluteRotation;
        public bool IsAbsoluteRotation
        {
            get => _IsAbsoluteRotation;
            set
            {
                if (_IsAbsoluteRotation == value)
                    return;
                _IsAbsoluteRotation = value;
                UpdateTransform();
            }
        }

        private bool _IsAbsoluteTranslation;
        public bool IsAbsoluteTranslation
        {
            get => _IsAbsoluteTranslation;
            set
            {
                if (_IsAbsoluteTranslation == value)
                    return;
                _IsAbsoluteTranslation = value;
                UpdateTransform();
            }
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

            // TODO: respect every component alone
            if (_IsAbsoluteScale && _IsAbsoluteRotation && _IsAbsoluteTranslation)
            {
                return trans;
            }

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

        public virtual void OnScreenMouseMove(MouseMoveArgs e)
        {
        }

        public virtual void OnScreenMouseDown(MouseButtonArgs e)
        {
        }

        public virtual void OnScreenMouseUp(MouseButtonArgs e)
        {
        }

        public event Action<MouseMoveArgs> MouseMove;
        public event Action<MouseButtonArgs> MouseUp;
        public event Action<MouseButtonArgs> MouseDown;
        public event Action<MouseButtonArgs> Click;

        public virtual void OnMouseMove(MouseMoveArgs e)
        {
            MouseMove?.Invoke(e);
        }

        public virtual void OnMouseDown(MouseButtonArgs e)
        {
            MouseDown?.Invoke(e);
        }

        public virtual void OnMouseUp(MouseButtonArgs e)
        {
            MouseUp?.Invoke(e);

            if (e.Handled)
                return;

            OnMouseClick(e);
        }

        public virtual void OnMouseClick(MouseButtonArgs e)
        {
            Click?.Invoke(e);
        }

        public int Level => ParentComponents.Count;

        internal override void DumpInfo(bool list)
        {
            Log.ForContext("DumpInfo").Info(new string(' ', (Level + 1) * 2) + "{Type} #{Id} {Name}", GetType().Name, ObjectId, Name);
            if (list)
                VisitChilds<GameObject>(a => a.DumpInfo(false));
        }

        public virtual Box3 WorldBounds { get; protected set; }
        public virtual Box3 LocalBounds { get; protected set; }

        public void UpdateWorldBounds()
        {
            UpdateWorldBounds(LocalToWorld());
        }

        public void UpdateWorldBounds(Matrix4 localToWorld)
        {
            var box = LocalBounds;
            var min = Vector3.TransformPosition(box.Min, localToWorld);
            var max = Vector3.TransformPosition(box.Max, localToWorld);
            WorldBounds = new Box3(min, max);
        }

    }
}
