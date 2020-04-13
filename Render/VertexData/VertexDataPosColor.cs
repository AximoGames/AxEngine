// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPosColor : IVertexPosition3, IVertexColor
    {
        public Vector3 Position;
        public Vector4 Color;

        public VertexDataPosColor(Vector3 position)
        {
            Position = position;
            Color = new Vector4();
        }

        public VertexDataPosColor(Vector3 position, Vector3 color)
        {
            Position = position;
            Color = new Vector4(color.X, color.Y, color.Z, 1.0f);
        }

        public VertexDataPosColor(Vector3 position, Vector4 color)
        {
            Position = position;
            Color = color;
        }

        Vector3 IVertexPosition3.Position { get => Position; set => Position = value; }
        Vector4 IVertexColor.Color { get => Color; set => Color = value; }
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPosColor> list, Vector3 position, Vector4 color)
        {
            list.Add(new VertexDataPosColor(position, color));
        }
    }
}
