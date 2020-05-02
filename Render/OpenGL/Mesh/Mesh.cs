// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Aximo.Render
{
    public abstract class Mesh
    {
        public Mesh() { }
        public Mesh(Mesh3 meshData)
        {
            SetMesh(meshData);
        }

        public Mesh(Mesh3 meshData, Material material)
        {
            SetMesh(meshData);
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

        public void SetMesh(Mesh3 mesh)
        {
            MeshData = mesh;
            MeshData2 = mesh.GetMeshData();
        }

        public List<Material> Materials = new List<Material>();
        public Mesh3 MeshData { get; private set; }
        internal MeshData MeshData2 { get; private set; }

        public int VertexCount => MeshData?.VertexCount ?? 0;
    }
}
