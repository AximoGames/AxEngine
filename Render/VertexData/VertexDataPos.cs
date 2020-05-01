// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
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
    }
}
