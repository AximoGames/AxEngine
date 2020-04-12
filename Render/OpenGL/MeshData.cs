// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenToolkit;

namespace Aximo.Render
{
    public enum AxPrimitiveType
    {
        Triangles,
        Lines,
    }

    public abstract class MeshData
    {
        public VertexLayoutDefinition Layout { get; protected set; }
        public abstract BufferData1D Data { get; }
        public virtual int VertexCount { get; protected set; }
        public BufferData1D<ushort> Indicies { get; protected set; }
        public virtual int IndiciesCount { get; protected set; }

        public VertexLayoutBinded BindLayoutToShader(Shader shader) => Layout.BindToShader(shader);
        public AxPrimitiveType PrimitiveType { get; protected set; }
    }

    public class MeshData<T> : MeshData
    {
        protected BufferData1D<T> _Data;

        public override BufferData1D Data => _Data;

        public MeshData()
        {
            Layout = VertexLayoutDefinition.CreateDefinitionFromVertexStruct<T>();
        }

        public MeshData(VertexLayoutDefinition layoutDefinition)
        {
            Layout = layoutDefinition;
        }

        public MeshData(VertexLayoutDefinition layoutDefinition, BufferData1D<T> data, BufferData1D<ushort> indicies = null, AxPrimitiveType primitiveType = AxPrimitiveType.Triangles)
        {
            Layout = layoutDefinition;
            SetData(data, indicies);
            PrimitiveType = primitiveType;
        }

        public MeshData(Type layoutDefinitionType, BufferData1D<T> data, BufferData1D<ushort> indicies = null, AxPrimitiveType primitiveType = AxPrimitiveType.Triangles)
        {
            Layout = VertexLayoutDefinition.CreateDefinitionFromVertexStruct(layoutDefinitionType);
            SetData(data, indicies);
            PrimitiveType = primitiveType;
        }

        public MeshData(BufferData1D<T> data, BufferData1D<ushort> indicies = null, AxPrimitiveType primitiveType = AxPrimitiveType.Triangles) : this()
        {
            SetData(data, indicies);
            PrimitiveType = primitiveType;
        }

        public void SetData(BufferData1D<T> data, BufferData1D<ushort> indicies = null)
        {
            _Data = data;
            Indicies = indicies;
            VertexCount = data == null ? 0 : data.Length;
            IndiciesCount = indicies == null ? 0 : indicies.Length;
        }
    }
}
