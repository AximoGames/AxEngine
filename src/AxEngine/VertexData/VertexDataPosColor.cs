using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit.Mathematics;

namespace AxEngine
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct VertexDataPosColor
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
    }

    public static partial class EngineExtensions
    {
        public static void Add(this IList<VertexDataPosColor> list, Vector3 position, Vector4 color)
        {
            list.Add(new VertexDataPosColor(position, color));
        }
    }

}
