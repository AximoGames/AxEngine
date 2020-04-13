// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public interface IPrimitive
    {
        int Count { get; }
        void CopyTo(ICollection<IPrimitive> destination);
    }

    public interface IPrimitive<T> : IPrimitive
    {
        T this[int index] { get; set; }
        void CopyTo(ICollection<IPrimitive<T>> destination);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Polygon<T> : IPrimitive<T>
    {
        public T Vertex0;
        public T Vertex1;
        public T Vertex2;

        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Vertex0;
                    case 1:
                        return Vertex1;
                    case 2:
                        return Vertex2;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Vertex0 = value;
                        return;
                    case 1:
                        Vertex1 = value;
                        return;
                    case 2:
                        Vertex2 = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public int Count => 3;

        public void CopyTo(ICollection<IPrimitive<T>> destination)
        {
            for (var i = 0; i < Count; i++)
                destination.Add((IPrimitive<T>)this[i]);
        }

        public void CopyTo(ICollection<IPrimitive> destination)
        {
            for (var i = 0; i < Count; i++)
                destination.Add((IPrimitive)this[i]);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Quad<T> : IPrimitive<T>
    {
        public T Vertex0;
        public T Vertex1;
        public T Vertex2;
        public T Vertex3;

        public T this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Vertex0;
                    case 1:
                        return Vertex1;
                    case 2:
                        return Vertex2;
                    case 3:
                        return Vertex3;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Vertex0 = value;
                        return;
                    case 1:
                        Vertex1 = value;
                        return;
                    case 2:
                        Vertex2 = value;
                        return;
                    case 3:
                        Vertex3 = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public int Count => 4;

        public void CopyTo(ICollection<IPrimitive<T>> destination)
        {
            for (var i = 0; i < Count; i++)
                destination.Add((IPrimitive<T>)this[i]);
        }

        public void CopyTo(ICollection<IPrimitive> destination)
        {
            for (var i = 0; i < Count; i++)
                destination.Add((IPrimitive)this[i]);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPosNormalUV : IVertex, IVertexPosition3, IVertexNormal, IVertexUV
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        public VertexDataPosNormalUV(Vector3 position, Vector3 normal, Vector2 uv)
        {
            Position = position;
            Normal = normal;
            UV = uv;
        }

        Vector3 IVertexPosition3.Position { get => Position; set => Position = value; }
        Vector3 IVertexNormal.Normal { get => Normal; set => Normal = value; }
        Vector2 IVertexUV.UV { get => UV; set => UV = value; }
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPosNormalUV> list, Vector3 position, Vector3 normal, Vector2 uv)
        {
            list.Add(new VertexDataPosNormalUV(position, normal, uv));
        }
    }
}
