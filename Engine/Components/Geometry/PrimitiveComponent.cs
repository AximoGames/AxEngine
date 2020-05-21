// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Aximo.Render;

namespace Aximo.Engine.Components.Geometry
{
    /// <summary>
    /// Represents a drawable object.
    /// </summary>
    public class PrimitiveComponent : SceneComponent
    {
        internal IRenderableObject RenderableObject;

        public bool CastShadow { get; set; }

        /// <summary>
        /// Higher number means, more near to the front.
        /// </summary>
        public int DrawPriority { get; set; }

        public virtual PrimitiveSceneProxy CreateProxy()
        {
            return new PrimitiveSceneProxy(this);
        }

        private List<GameMaterial> _Materials;
        public ICollection<GameMaterial> Materials { get; private set; }

        public PrimitiveComponent()
        {
            _Materials = new List<GameMaterial>();
            Materials = new ReadOnlyCollection<GameMaterial>(_Materials);
        }

        public void AddMaterial(GameMaterial material)
        {
            _Materials.Add(material);
            material.AddRef(this);
        }

        public void AddMaterial(GameMaterial material, int index)
        {
            _Materials.Insert(index, material);
            material.AddRef(this);
        }

        public void RemoveMaterial(GameMaterial material)
        {
            _Materials.Remove(material);
            material.RemoveRef(this);
        }

        public GameMaterial Material
        {
            get
            {
                return _Materials.FirstOrDefault();
            }
            set
            {
                if (value == null)
                    throw new InvalidOperationException();

                if (_Materials.Count == 0)
                {
                    AddMaterial(value);
                }
                else
                {
                    RemoveMaterial(_Materials[0]);
                    AddMaterial(value);
                }
            }
        }

        internal override void PropagateChangesUp()
        {
            foreach (var mat in _Materials)
                mat.PropagateChanges();

            base.PropagateChangesUp();
        }

        internal override void SyncChanges()
        {
            foreach (var mat in _Materials)
                mat.SyncChanges();

            base.SyncChanges();
        }

        public override void Visit<T>(Action<T> action, Func<T, bool> visitChilds = null)
        {
            base.Visit(action, visitChilds);

            if (visitChilds != null)
                if (this is T obj)
                    if (!visitChilds.Invoke(obj))
                        return;

            foreach (var mat in _Materials)
                mat.Visit(action, visitChilds);
        }
    }
}
