// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPosUV : IVertexPosUV
    {
        public Vector3 Position;
        public Vector2 UV;

        public VertexDataPosUV(Vector3 position, Vector2 uv)
        {
            Position = position;
            UV = uv;
        }

        Vector3 IVertexPosition<Vector3>.Position { get => Position; set => Position = value; }
        Vector2 IVertexUV.UV { get => UV; set => UV = value; }

        public void Set(IVertexPosUV source)
        {
            Position = source.Position;
            UV = source.UV;
        }

        public void Set(VertexDataPosUV source)
        {
            Position = source.Position;
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

        public VertexDataPosUV Clone() => new VertexDataPosUV(Position, UV);
        IVertexPosition3 IVertexPosition3.Clone() => Clone();
        IVertexPosition<Vector3> IVertexPosition<Vector3>.Clone() => Clone();
        IVertex IVertex.Clone() => Clone();
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPosUV> list, Vector3 position, Vector2 uv)
        {
            list.Add(new VertexDataPosUV(position, uv));
        }
    }
}
