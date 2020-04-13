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

        public Quad(T[] vertices)
        {
            Vertex0 = vertices[0];
            Vertex1 = vertices[1];
            Vertex2 = vertices[2];
            Vertex3 = vertices[3];
        }

        public Quad(T vertex0, T vertex1, T vertex2, T vertex3)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
            Vertex2 = vertex2;
            Vertex3 = vertex3;
        }

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

        public Polygon<T>[] ToPolygons()
        {
            var p1 = new Polygon<T>();
            p1.Vertex0 = Vertex0;
            p1.Vertex1 = Vertex1;
            p1.Vertex2 = Vertex2;

            var p2 = new Polygon<T>();
            p1.Vertex0 = Vertex2;
            p1.Vertex1 = Vertex3;
            p1.Vertex2 = Vertex0;

            return new Polygon<T>[] { p1, p2 };
        }

        public T[] ToVertices()
        {
            return new T[] { Vertex0, Vertex1, Vertex2, Vertex3 };
        }

        public T[] ToPolygonVertices()
        {
            return new T[] { Vertex0, Vertex1, Vertex2, Vertex2, Vertex3, Vertex0 };
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPosNormalUV : IVertex, IVertexPosition3, IVertexNormal, IVertexUV
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        public static Quad<VertexDataPosNormalUV> DefaultQuad
        {
            get
            {
                var result = new Quad<VertexDataPosNormalUV>();
                result.SetPosition(VertexDataPos.DefaultQuad);
                result.SetNormal(Vector3.UnitZ);
                result.MapUV();
                return result;
            }

        }

        //public static readonly Polygon<VertexDataPosNormalUV> DefaultPolygon = new Polygon<VertexDataPosNormalUV>(
        //    new VertexDataPosNormalUV(new Vector3(-1f, -1f, 0.0f), Vector3.UnitZ, new Vector2(0.0f, 1.0f)),
        //    new VertexDataPosNormalUV(new Vector3(1f, -1f, 0.0f), Vector3.UnitZ, new Vector2(1.0f, 1.0f)),
        //    new VertexDataPosNormalUV(new Vector3(1f, 1f, 0.0f), Vector3.UnitZ, new Vector2(1.0f, 0.0f)));

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

        public static void SetNormal(this ref Quad<VertexDataPosNormalUV> quad, Vector3 normal)
        {
            quad.Vertex0.Normal = normal;
            quad.Vertex1.Normal = normal;
            quad.Vertex2.Normal = normal;
            quad.Vertex3.Normal = normal;
        }

        public static void MapUV(this ref Quad<VertexDataPosNormalUV> quad, Vector2 fromMin, Vector2 fromMax, Vector2 toMin, Vector2 toMax)
        {
            quad.Vertex0.UV = AxMath.Map(quad.Vertex0.Position.Xy, fromMin, fromMax, toMin, toMax);
            quad.Vertex1.UV = AxMath.Map(quad.Vertex1.Position.Xy, fromMin, fromMax, toMin, toMax);
            quad.Vertex2.UV = AxMath.Map(quad.Vertex2.Position.Xy, fromMin, fromMax, toMin, toMax);
            quad.Vertex3.UV = AxMath.Map(quad.Vertex3.Position.Xy, fromMin, fromMax, toMin, toMax);
        }

        public static void MapUV(this ref Quad<VertexDataPosNormalUV> quad, Vector2 fromMin, Vector2 fromMax)
        {
            MapUV(ref quad, fromMin, fromMax, new Vector2(0, 1), new Vector2(1, 0));
        }

        public static void MapUV(this ref Quad<VertexDataPosNormalUV> quad)
        {
            MapUV(ref quad, new Vector2(-1, -1), new Vector2(1, 1), new Vector2(0, 1), new Vector2(1, 0));
        }

        public static void SetPosition(this ref Quad<VertexDataPosNormalUV> quad, Quad<VertexDataPos> source)
        {
            quad.Vertex0.Position = source.Vertex0.Position;
            quad.Vertex1.Position = source.Vertex1.Position;
            quad.Vertex2.Position = source.Vertex2.Position;
            quad.Vertex3.Position = source.Vertex3.Position;
        }
    }
}
