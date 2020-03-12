// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Aximo.Render;

namespace Aximo.Engine
{
    public class StaticMesh : StreamableRenderAsset
    {

        public StaticMeshSourceModel SourceModel { get; private set; }

        public void SetSourceModel(StaticMeshSourceModel model)
        {

        }

        private List<Material> _Materials;
        public ICollection<Material> Materials { get; private set; }

        public SceneComponent Parent { get; private set; }

        public StaticMesh()
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

        public int GetNumVertices(int lod)
        {
            throw new NotImplementedException();
        }

    }

}
