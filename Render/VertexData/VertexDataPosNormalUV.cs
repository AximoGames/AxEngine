// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPosNormalUV : IVertexPosNormalUV
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        /// <summary>
        /// Returns a Quad, grounded to XY, facing Z-Up
        /// </summary>
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

        public VertexDataPosNormalUV(Vector3 position, Vector3 normal, Vector2 uv)
        {
            Position = position;
            Normal = normal;
            UV = uv;
        }

        Vector3 IVertexPosition<Vector3>.Position { get => Position; set => Position = value; }
        Vector3 IVertexNormal.Normal { get => Normal; set => Normal = value; }
        Vector2 IVertexUV.UV { get => UV; set => UV = value; }

        public void Set(IVertexPosNormalUV source)
        {
            Position = source.Position;
            Normal = source.Normal;
            UV = source.UV;
        }
        public void Set(VertexDataPosNormalUV source)
        {
            Position = source.Position;
            Normal = source.Normal;
            UV = source.UV;
        }

        public void SetPosition(IVertexPosition3 source)
        {
            Position = source.Position;
        }
        public void SetPosition(Vector3 source)
        {
            Position = source;
        }

        public VertexDataPosNormalUV Clone()
        {
            return new VertexDataPosNormalUV(Position, Normal, UV);
        }

        IVertexPosition3 IVertexPosition3.Clone()
        {
            return Clone();
        }

        IVertexPosition<Vector3> IVertexPosition<Vector3>.Clone()
        {
            return Clone();
        }

        IVertexNormal IVertexNormal.Clone()
        {
            return Clone();
        }

        IVertex IVertex.Clone()
        {
            return Clone();
        }
    }

    //public static class Operations
    //{
    //    public static void CopyPosition<TSource, TDestination>(in TSource source, ref TDestination destination)
    //        where TSource : IVertexPosition3
    //        where TDestination : IVertexPosition3
    //    {
    //        destination.Position = source.Position;
    //    }
    //    private static void Test()
    //    {
    //        var source = new VertexDataPosNormalUV();
    //        var dest = new VertexDataPosNormalUV();

    //        IVertexPosition3 source2 = new VertexDataPosNormalColor();
    //        IVertexPosition3 dest2 = new VertexDataPosNormalColor();

    //        CopyPosition(dest, ref source);
    //        CopyPosition(dest2, ref source2);
    //        CopyPosition(dest, ref source2);
    //        CopyPosition(dest2, ref source);
    //    }
    //}

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

        public static void SetPosition<TSource>(this ref Quad<VertexDataPosNormalUV> quad, Quad<TSource> source)
        {
            if (typeof(IVertexPosition3).IsAssignableFrom(typeof(TSource)))
            {
                quad.Vertex0.Position = ((IVertexPosition3)source[0]).Position;
                quad.Vertex1.Position = ((IVertexPosition3)source[1]).Position;
                quad.Vertex2.Position = ((IVertexPosition3)source[2]).Position;
                quad.Vertex3.Position = ((IVertexPosition3)source[3]).Position;
            }
            else if (typeof(IVertexPosition2).IsAssignableFrom(typeof(TSource)))
            {
                quad.Vertex0.Position.Xy = ((IVertexPosition2)source[0]).Position;
                quad.Vertex1.Position.Xy = ((IVertexPosition2)source[1]).Position;
                quad.Vertex2.Position.Xy = ((IVertexPosition2)source[2]).Position;
                quad.Vertex3.Position.Xy = ((IVertexPosition2)source[3]).Position;
            }
        }
    }
}
