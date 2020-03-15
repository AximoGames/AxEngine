// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Aximo.Render
{

    public abstract class MeshData
    {
        public VertexLayoutDefinition Layout { get; protected set; }
        public abstract Array Data { get; }
        public virtual int VertexCount { get; protected set; }
        public ushort[] Indicies { get; protected set; }
        public virtual int IndiciesCount { get; protected set; }

        public VertexLayoutBinded BindLayoutToShader(Shader shader) => Layout.BindToShader(shader);
    }

    public class MeshData<T> : MeshData
    {
        protected T[] _Data;

        public override Array Data => _Data;

        public MeshData()
        {
            Layout = VertexLayoutDefinition.CreateDefinitionFromVertexStruct<T>();
        }

        public MeshData(VertexLayoutDefinition layoutDefinition)
        {
            Layout = layoutDefinition;
        }

        public MeshData(VertexLayoutDefinition layoutDefinition, T[] data, ushort[] indicies = null)
        {
            Layout = layoutDefinition;
            SetData(data, indicies);
        }

        public MeshData(Type layoutDefinitionType, T[] data, ushort[] indicies = null)
        {
            Layout = VertexLayoutDefinition.CreateDefinitionFromVertexStruct(layoutDefinitionType);
            SetData(data, indicies);
        }

        public MeshData(T[] data, ushort[] indicies = null) : this()
        {
            SetData(data, indicies);
        }

        public void SetData(T[] data, ushort[] indicies = null)
        {
            _Data = data;
            Indicies = indicies;
            VertexCount = data == null ? 0 : data.Length;
            IndiciesCount = indicies == null ? 0 : indicies.Length;
        }

    }

}
