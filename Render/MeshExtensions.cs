// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Aximo.VertexData;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render.OpenGL
{
    public static class MeshExtensions
    {
        public static MeshData GetMeshData(this Mesh m, int materialId)
        {
            if (m.Name == "Bomb")
            {
                var s = "";
            }

            // return ToPrimitive(PrimitiveType, materialId).GetMeshData();
            var mesh = m.CloneEmpty();
            mesh.AddMesh(m, materialId);

            if (mesh.PrimitiveType >= MeshFaceType.Quad)
                mesh = mesh.ToPrimitive(MeshFaceType.Triangle);

            return mesh.GetMeshData();
        }

        private static MeshData GetMeshData(this Mesh m)
        {
            if (m.IsCompatible<IVertexPosNormalUV>())
                return new MeshData<VertexDataPosNormalUV>(m.View<IVertexPosNormalUV>().ToBuffer<VertexDataPosNormalUV>(), m.GetIndiciesBuffer<ushort>(), m.GetPrimitiveType());
            if (m.IsCompatible<IVertexPosNormalColor>())
                return new MeshData<VertexDataPosNormalColor>(m.View<IVertexPosNormalColor>().ToBuffer<VertexDataPosNormalColor>(), m.GetIndiciesBuffer<ushort>(), m.GetPrimitiveType());
            if (m.IsCompatible<IVertexPosColor>())
                return new MeshData<VertexDataPosColor>(m.View<IVertexPosColor>().ToBuffer<VertexDataPosColor>(), m.GetIndiciesBuffer<ushort>(), m.GetPrimitiveType());
            if (m.IsCompatible<IVertexPos2UV>())
                return new MeshData<VertexDataPos2UV>(m.View<IVertexPos2UV>().ToBuffer<VertexDataPos2UV>(), m.GetIndiciesBuffer<ushort>(), m.GetPrimitiveType());

            throw new NotSupportedException();
        }

        public static MeshData<T> ToMeshData<T>(this Mesh m)
            where T : struct, IVertex
        {
            var polyMesh = m.ToPrimitive();
            var data = new T[polyMesh.VertexCount];
            for (var i = 0; i < polyMesh.VertexCount; i++)
                data[i] = polyMesh.ToMeshData<T>(i);
            var indicies = polyMesh.Indicies.Select(index => (ushort)index).ToArray();
            return new MeshData<T>(new BufferData1D<T>(data), new BufferData1D<ushort>(indicies));
        }

        public static T ToMeshData<T>(this Mesh m, int index)
            where T : struct, IVertex
        {
            Vector3 pos = Vector3.Zero;
            Vector3 normal = Vector3.UnitZ;
            Vector2 uv = Vector2.Zero;

            foreach (var c in m.Components)
            {
                if (c is MeshPosition3Component c1)
                    pos = c1.Values[index];
            }

            if (typeof(T) == typeof(VertexDataPosNormalUV))
                return (T)(object)new VertexDataPosNormalUV(pos, normal, uv);

            throw new NotSupportedException();
        }
    }
}
