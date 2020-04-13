// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPos2UV : IVertex, IVertexPosition2, IVertexUV
    {
        public Vector2 Position;
        public Vector2 UV;

        public VertexDataPos2UV(Vector2 position, Vector2 uv)
        {
            Position = position;
            UV = uv;
        }

        Vector2 IVertexPosition2.Position { get => Position; set => Position = value; }
        Vector2 IVertexUV.UV { get => UV; set => UV = value; }
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPos2UV> list, Vector2 position, Vector2 uv)
        {
            list.Add(new VertexDataPos2UV(position, uv));
        }
    }
}
