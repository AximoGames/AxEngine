// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
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
