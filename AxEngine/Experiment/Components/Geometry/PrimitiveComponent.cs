// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Aximo.Render;

namespace Aximo.Engine
{

    public class PrimitiveComponent : SceneComponent
    {

        internal IRenderableObject RenderableObject;

        public bool CastShadow { get; set; }

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
        }

        public void AddMaterial(GameMaterial material, int index)
        {
            _Materials.Insert(index, material);
        }

        public void RemoveMaterial(GameMaterial material)
        {
            _Materials.Remove(material);
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
                    _Materials.Add(value);
                else
                    _Materials[0] = value;
            }
        }

        internal override void PropagateChanges()
        {
            foreach (var mat in _Materials)
                mat.PropagateChanges();

            base.PropagateChanges();
        }

        internal override void SyncChanges()
        {
            foreach (var mat in _Materials)
                mat.SyncChanges();

            base.SyncChanges();
        }

    }

}
