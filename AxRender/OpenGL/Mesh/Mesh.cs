// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Aximo.Render
{

    public abstract class Mesh
    {

        public Mesh() { }
        public Mesh(MeshData meshData)
        {
            MeshData = meshData;
        }

        public Mesh(MeshData meshData, Material material)
        {
            MeshData = meshData;
            if (material == null)
                throw new InvalidOperationException();

            Materials.Add(material);
        }

        public Material Material
        {
            get
            {
                if (Materials.Count == 0)
                    return null;
                return Materials[0];
            }
            set
            {
                if (value == null)
                    throw new InvalidOperationException();

                if (Materials.Count == 0)
                    Materials.Add(value);
                else
                    Materials[0] = value;
            }
        }

        public List<Material> Materials = new List<Material>();
        public MeshData MeshData { get; private set; }

        public int VertexCount => MeshData?.VertexCount ?? 0;
    }

}
