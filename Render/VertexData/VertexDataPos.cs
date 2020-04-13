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

        public static Quad<VertexDataPos> DefaultQuad => new Quad<VertexDataPos>(
            new VertexDataPos(new Vector3(-1f, -1f, 0.0f)),
            new VertexDataPos(new Vector3(1f, -1f, 0.0f)),
            new VertexDataPos(new Vector3(1f, 1f, 0.0f)),
            new VertexDataPos(new Vector3(-1f, 1f, 0.0f)));

        public VertexDataPos(Vector3 position)
        {
            Position = position;
        }

        Vector3 IVertexPosition3.Position { get => Position; set => Position = value; }
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPos> list, Vector3 position)
        {
            list.Add(new VertexDataPos(position));
        }
    }
}
