// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.VertexData
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPos : IVertexPosition3
    {
        public Vector3 Position;

        // Vertex order:
        //
        // 3  2
        //
        // 0  1

        public static Quad<VertexDataPos> DefaultQuad
        {
            get
            {
                var result = new Quad<VertexDataPos>();
                result.SetPosition(VertexDataPos2.DefaultQuad);
                return result;
            }
        }

        public static Quad<VertexDataPos> WallQuad
        {
            get
            {
                var result = new Quad<VertexDataPos>();
                result.SetPosition(VertexDataPos2.DefaultQuad);
                result.Rotate(new Rotor3(Vector3.UnitZ, -Vector3.UnitY));
                result.RoundSmooth();
                return result;
            }
        }

        public VertexDataPos(Vector3 position)
        {
            Position = position;
        }

        Vector3 IVertexPosition<Vector3>.Position { get => Position; set => Position = value; }

        public void SetPosition(IVertexPosition3 source)
        {
            Position = source.Position;
        }
        public void SetPosition(Vector3 source)
        {
            Position = source;
        }

        public VertexDataPos Clone() => new VertexDataPos(Position);
        IVertexPosition3 IVertexPosition3.Clone() => Clone();
        IVertexPosition<Vector3> IVertexPosition<Vector3>.Clone() => Clone();
        IVertex IVertex.Clone() => Clone();
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPos> list, Vector3 position)
        {
            list.Add(new VertexDataPos(position));
        }

        public static void SetPosition(this ref Quad<VertexDataPos> quad, Quad<VertexDataPos2> source)
        {
            quad.Vertex0.Position.Xy = source.Vertex0.Position;
            quad.Vertex1.Position.Xy = source.Vertex1.Position;
            quad.Vertex2.Position.Xy = source.Vertex2.Position;
            quad.Vertex3.Position.Xy = source.Vertex3.Position;
        }

        public static void Rotate(this ref Quad<VertexDataPos> quad, Rotor3 q)
        {
            for (var i = 0; i < quad.Count; i++)
            {
                var v = quad[i];
                v.Position = Rotor3.Rotate(q, v.Position);
                quad[i] = v;
            }
        }

        public static void Round(this ref Quad<VertexDataPos> quad, int digits)
        {
            for (var i = 0; i < quad.Count; i++)
            {
                var v = quad[i];
                v.Position = v.Position.Round(digits);
                quad[i] = v;
            }
        }

        public static void RoundSmooth(this ref Quad<VertexDataPos> quad)
        {
            Round(ref quad, 6);
        }
    }
}
