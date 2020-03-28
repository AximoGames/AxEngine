// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

    public abstract class BufferData
    {
        public abstract int Length { get; }
        public abstract int ElementSize { get; }

        public abstract GCHandle Createhandle();

        public static BufferData1D<T> Create<T>(T[] data)
        {
            return new BufferData1D<T>(data);
        }

        public static BufferData2D<T> Create<T>(T[,] data)
        {
            return new BufferData2D<T>(data);
        }

        public static BufferData3D<T> Create<T>(T[,,] data)
        {
            return new BufferData3D<T>(data);
        }
    }

    public abstract class BufferData1D : BufferData
    {
        public abstract int SizeX { get; }
    }

    public class BufferData1D<T> : BufferData1D
    {

        public BufferData1D(T[] data)
        {
            SetData(data);
        }

        public void SetData(T[] data)
        {
            _Data = data;
            _Length = data.Length;
            _ElementSize = Marshal.SizeOf<T>();
        }

        public override GCHandle Createhandle()
        {
            return GCHandle.Alloc(_Data, GCHandleType.Pinned);
        }

        private T[] _Data;

        public T this[int index]
        {
            get { return _Data[index]; }
            set { _Data[index] = value; }
        }

        private int _Length;
        public override int Length => _Length;

        public override int SizeX => _Length;

        private int _ElementSize;
        public override int ElementSize => _ElementSize;
    }

    public abstract class BufferData2D : BufferData
    {
        public abstract int SizeX { get; }
        public abstract int SizeY { get; }
    }

    public class BufferData2D<T> : BufferData2D
    {
        public BufferData2D(T[,] data)
        {
            SetData(data);
        }

        public void SetData(T[,] data)
        {
            _Data = data;
            _Length = data.Length;
            _SizeX = data.GetUpperBound(0);
            _SizeY = data.GetUpperBound(1);
            _ElementSize = Marshal.SizeOf<T>();
        }

        private T[,] _Data;

        public T this[int x, int y]
        {
            get { return _Data[x, y]; }
            set { _Data[x, y] = value; }
        }

        private int _Length;
        public override int Length => _Length;

        private int _SizeX;
        public override int SizeX => _SizeX;

        private int _SizeY;
        public override int SizeY => _SizeY;

        public override GCHandle Createhandle()
        {
            return GCHandle.Alloc(_Data, GCHandleType.Pinned);
        }

        private int _ElementSize;
        public override int ElementSize => _ElementSize;
    }

    public abstract class BufferData3D : BufferData
    {
        public abstract int SizeX { get; }
        public abstract int SizeY { get; }
        public abstract int SizeZ { get; }
    }

    public class BufferData3D<T> : BufferData3D
    {
        public BufferData3D(T[,,] data)
        {
            SetData(data);
        }

        public void SetData(T[,,] data)
        {
            _Data = data;
            _Length = data.Length;
            _SizeX = data.GetUpperBound(0);
            _SizeY = data.GetUpperBound(1);
            _SizeZ = data.GetUpperBound(2);
            _ElementSize = Marshal.SizeOf<T>();
        }

        private T[,,] _Data;

        public T this[int x, int y, int z]
        {
            get { return _Data[x, y, z]; }
            set { _Data[x, y, z] = value; }
        }

        private int _Length;
        public override int Length => _Length;

        private int _SizeX;
        public override int SizeX => _SizeX;

        private int _SizeY;
        public override int SizeY => _SizeY;

        private int _SizeZ;
        public override int SizeZ => _SizeZ;

        public override GCHandle Createhandle()
        {
            return GCHandle.Alloc(_Data, GCHandleType.Pinned);
        }

        private int _ElementSize;
        public override int ElementSize => _ElementSize;
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
