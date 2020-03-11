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

        private List<IMaterialInterface> _Materials;
        public ICollection<IMaterialInterface> Materials { get; private set; }

        public SceneComponent Parent { get; private set; }

        public StaticMesh()
        {
            _Materials = new List<IMaterialInterface>();
            Materials = new ReadOnlyCollection<IMaterialInterface>(_Materials);
        }

        public void AddMaterial(IMaterialInterface material)
        {
            _Materials.Add(material);
        }

        public void RemoveMaterial(IMaterialInterface material)
        {
            _Materials.Remove(material);
        }

        public int GetNumVertices(int lod)
        {
            throw new NotImplementedException();
        }

    }

}
