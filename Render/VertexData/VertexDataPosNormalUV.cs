// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPosNormalUV
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        public VertexDataPosNormalUV(Vector3 position, Vector3 normal, Vector2 uv)
        {
            Position = position;
            Normal = normal;
            UV = uv;
        }
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPosNormalUV> list, Vector3 position, Vector3 normal, Vector2 uv)
        {
            list.Add(new VertexDataPosNormalUV(position, normal, uv));
        }
    }
}
