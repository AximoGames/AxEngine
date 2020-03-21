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

        private List<Material> _Materials;
        public ICollection<Material> Materials { get; private set; }

        public PrimitiveComponent()
        {
            _Materials = new List<Material>();
            Materials = new ReadOnlyCollection<Material>(_Materials);
        }

        public void AddMaterial(Material material)
        {
            _Materials.Add(material);
        }

        public void RemoveMaterial(Material material)
        {
            _Materials.Remove(material);
        }

        // internal override void PropagateChanges()
        // {
        //     foreach (var mat in _Materials)
        //         mat.pr();
        // }

        // internal override void SyncChanges()
        // {
        //     foreach (var mat in _Materials)
        //         mat.SyncChanges();

        //     base.SyncChanges();
        // }

    }

}
