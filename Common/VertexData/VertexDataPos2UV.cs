﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit.Mathematics;

namespace Aximo.VertexData
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPos2UV : IVertexPos2UV
    {
        public Vector2 Position;
        public Vector2 UV;

        public static Quad<VertexDataPos2UV> DefaultQuad
        {
            get
            {
                var result = new Quad<VertexDataPos2UV>();
                result.SetPosition(VertexDataPos2.DefaultQuad);
                result.MapUV();
                return result;
            }
        }

        public static Quad<VertexDataPos2UV> DefaultQuadInvertedUV
        {
            get
            {
                var result = new Quad<VertexDataPos2UV>();
                result.SetPosition(VertexDataPos2.DefaultQuad);
                result.MapUV();
                result.MapInvertUV();
                return result;
            }
        }

        public static Quad<VertexDataPos2UV> NDCQuadInvertedUV
        {
            get
            {
                var result = new Quad<VertexDataPos2UV>();
                result.SetPosition(VertexDataPos2.DefaultQuad);
                result.Scale(2f);
                result.MapUV();
                result.MapInvertUV();
                return result;
            }
        }

        public VertexDataPos2UV(Vector2 position, Vector2 uv)
        {
            Position = position;
            UV = uv;
        }

        Vector2 IVertexPosition<Vector2>.Position { get => Position; set => Position = value; }
        Vector2 IVertexUV.UV { get => UV; set => UV = value; }

        public void Set(IVertexPos2UV source)
        {
            Position = source.Position;
            UV = source.UV;
        }

        public void Set(VertexDataPos2UV source)
        {
            Position = source.Position;
            UV = source.UV;
        }

        public VertexDataPos2UV Clone() => new VertexDataPos2UV(Position, UV);
        IVertexPosition2 IVertexPosition2.Clone() => Clone();
        IVertexPosition<Vector2> IVertexPosition<Vector2>.Clone() => Clone();
        IVertex IVertex.Clone() => Clone();
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPos2UV> list, Vector2 position, Vector2 uv)
        {
            list.Add(new VertexDataPos2UV(position, uv));
        }

        public static void MapUV(this ref Quad<VertexDataPos2UV> quad, Vector2 fromMin, Vector2 fromMax, Vector2 toMin, Vector2 toMax)
        {
            quad.Vertex0.UV = AxMath.Map(quad.Vertex0.Position, fromMin, fromMax, toMin, toMax);
            quad.Vertex1.UV = AxMath.Map(quad.Vertex1.Position, fromMin, fromMax, toMin, toMax);
            quad.Vertex2.UV = AxMath.Map(quad.Vertex2.Position, fromMin, fromMax, toMin, toMax);
            quad.Vertex3.UV = AxMath.Map(quad.Vertex3.Position, fromMin, fromMax, toMin, toMax);
        }

        public static void MapUV(this ref Quad<VertexDataPos2UV> quad, Vector2 fromMin, Vector2 fromMax)
        {
            MapUV(ref quad, fromMin, fromMax, new Vector2(0, 1), new Vector2(1, 0));
        }

        public static void MapUV(this ref Quad<VertexDataPos2UV> quad)
        {
            MapUV(ref quad, quad.BottomLeft.Position, quad.TopRight.Position, new Vector2(0, 1), new Vector2(1, 0));
        }

        public static void MapInvertUV(this ref Quad<VertexDataPos2UV> quad)
        {
            quad.Vertex0.UV.Y = 1 - quad.Vertex0.UV.Y;
            quad.Vertex1.UV.Y = 1 - quad.Vertex1.UV.Y;
            quad.Vertex2.UV.Y = 1 - quad.Vertex2.UV.Y;
            quad.Vertex3.UV.Y = 1 - quad.Vertex3.UV.Y;
        }

        public static void Scale(this ref Quad<VertexDataPos2UV> quad, Vector2 scale)
        {
            quad.Vertex0.Position *= scale;
            quad.Vertex1.Position *= scale;
            quad.Vertex2.Position *= scale;
            quad.Vertex3.Position *= scale;
        }

        public static void Scale(this ref Quad<VertexDataPos2UV> quad, float scale)
        {
            Scale(ref quad, new Vector2(scale));
        }

        public static void Translate(this ref Quad<VertexDataPos2UV> quad, Vector2 value)
        {
            quad.Vertex0.Position += value;
            quad.Vertex1.Position += value;
            quad.Vertex2.Position += value;
            quad.Vertex3.Position += value;
        }

        public static void SetPosition<TSource>(this ref Quad<VertexDataPos2UV> quad, Quad<TSource> source)
        {
            if (typeof(IVertexPosition2).IsAssignableFrom(typeof(TSource)))
            {
                quad.Vertex0.Position = ((IVertexPosition2)source[0]).Position;
                quad.Vertex1.Position = ((IVertexPosition2)source[1]).Position;
                quad.Vertex2.Position = ((IVertexPosition2)source[2]).Position;
                quad.Vertex3.Position = ((IVertexPosition2)source[3]).Position;
            }
            else if (typeof(IVertexPosition3).IsAssignableFrom(typeof(TSource)))
            {
                quad.Vertex0.Position = ((IVertexPosition3)source[0]).Position.Xy;
                quad.Vertex1.Position = ((IVertexPosition3)source[1]).Position.Xy;
                quad.Vertex2.Position = ((IVertexPosition3)source[2]).Position.Xy;
                quad.Vertex3.Position = ((IVertexPosition3)source[3]).Position.Xy;
            }
        }
    }
}
