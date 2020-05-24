// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.VertexData
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPosNormalColor : IVertexPosNormalColor
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector4 Color;

        /// <summary>
        /// Returns a Quad, grounded to XY, facing Z-Up
        /// </summary>
        public static Quad<VertexDataPosNormalColor> DefaultQuad
        {
            get
            {
                var result = new Quad<VertexDataPosNormalColor>();
                result.SetPosition(VertexDataPos.DefaultQuad);
                result.SetNormal(Vector3.UnitZ);
                return result;
            }
        }

        public VertexDataPosNormalColor(Vector3 position, Vector3 normal, Vector4 color)
        {
            Position = position;
            Normal = normal;
            Color = color;
        }

        Vector3 IVertexPosition<Vector3>.Position { get => Position; set => Position = value; }
        Vector3 IVertexNormal.Normal { get => Normal; set => Normal = value; }
        Vector4 IVertexColor.Color { get => Color; set => Color = value; }

        public void SetPosition(IVertexPosition3 source)
        {
            Position = source.Position;
        }
        public void SetPosition(Vector3 source)
        {
            Position = source;
        }

        public void Set(IVertexPosNormalColor source)
        {
            Position = source.Position;
            Normal = source.Normal;
            Color = source.Color;
        }

        public void Set(VertexDataPosNormalColor source)
        {
            Position = source.Position;
            Normal = source.Normal;
            Color = source.Color;
        }

        public VertexDataPosNormalColor Clone()
        {
            return new VertexDataPosNormalColor(Position, Normal, Color);
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

        IVertexColor IVertexColor.Clone()
        {
            return Clone();
        }

        IVertex IVertex.Clone()
        {
            return Clone();
        }
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPosNormalColor> list, Vector3 position, Vector3 normal, Vector4 color)
        {
            list.Add(new VertexDataPosNormalColor(position, normal, color));
        }

        public static void SetNormal(this ref Quad<VertexDataPosNormalColor> quad, Vector3 normal)
        {
            quad.Vertex0.Normal = normal;
            quad.Vertex1.Normal = normal;
            quad.Vertex2.Normal = normal;
            quad.Vertex3.Normal = normal;
        }

        public static void SetNormal(this ref Polygon<VertexDataPosNormalColor> polygon, Vector3 normal)
        {
            polygon.Vertex0.Normal = normal;
            polygon.Vertex1.Normal = normal;
            polygon.Vertex2.Normal = normal;
        }

        public static void SetColor(this ref Quad<VertexDataPosNormalColor> quad, Vector4 color)
        {
            quad.Vertex0.Color = color;
            quad.Vertex1.Color = color;
            quad.Vertex2.Color = color;
            quad.Vertex3.Color = color;
        }

        public static void SetColor(this ref Polygon<VertexDataPosNormalColor> polygon, Vector4 color)
        {
            polygon.Vertex0.Color = color;
            polygon.Vertex1.Color = color;
            polygon.Vertex2.Color = color;
        }

        public static void SetPosition<TSource>(this ref Quad<VertexDataPosNormalColor> quad, Quad<TSource> source)
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
