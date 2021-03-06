﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Aximo.Render.OpenGL
{
    public abstract class InternalMesh
    {
        public InternalMesh() { }
        public InternalMesh(Mesh meshData)
        {
            SetMesh(meshData);
        }

        public InternalMesh(Mesh meshData, RendererMaterial material)
        {
            SetMesh(meshData);
            if (material == null)
                throw new InvalidOperationException();

            Materials.Add(material);
        }

        public bool HasData => Mesh != null;

        public RendererMaterial Material
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

        public void SetMesh(Mesh mesh)
        {
            Mesh = mesh;
            MeshDataList.Clear();
            if (mesh != null)
            {
                for (var i = 0; i < mesh.MaterialCount; i++)
                    MeshDataList.Add(mesh.GetMeshData(i));
            }
        }

        public List<RendererMaterial> Materials = new List<RendererMaterial>();
        public Mesh Mesh { get; private set; }
        private List<MeshData> MeshDataList { get; } = new List<MeshData>();

        internal MeshData GetMeshData(int materialId)
        {
            return MeshDataList[materialId];
        }

        public ReadOnlyCollection<int> MaterialIds => Array.AsReadOnly(Mesh.MaterialIds.ToArray());

        internal RendererMaterial GetMaterial(int materialId)
        {
            if (materialId >= Materials.Count)
                return RendererMaterial.GetDefault();

            return Materials[materialId];
        }
    }
}
