// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPos2 : IVertexPosition2
    {
        public Vector2 Position;

        // Vertex order:
        //
        // 3  2
        //
        // 0  1

        public static Quad<VertexDataPos2> DefaultQuad => new Quad<VertexDataPos2>(
            new VertexDataPos2(new Vector2(-1f, -1f)),
            new VertexDataPos2(new Vector2(1f, -1f)),
            new VertexDataPos2(new Vector2(1f, 1f)),
            new VertexDataPos2(new Vector2(-1f, 1f)));

        public VertexDataPos2(Vector2 position)
        {
            Position = position;
        }

        Vector2 IVertexPosition<Vector2>.Position { get => Position; set => Position = value; }

        public VertexDataPos2 Clone() => new VertexDataPos2(Position);
        IVertexPosition2 IVertexPosition2.Clone() => Clone();
        IVertexPosition<Vector2> IVertexPosition<Vector2>.Clone() => Clone();
        IVertex IVertex.Clone() => Clone();
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPos2> list, Vector2 position)
        {
            list.Add(new VertexDataPos2(position));
        }
    }
}
