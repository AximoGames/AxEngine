using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenTK;

namespace AxEngine
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
