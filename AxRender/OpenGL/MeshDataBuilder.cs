// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public static class MeshDataBuilder
    {
        public static MeshData Cube()
        {
            return new MeshData<float>(typeof(VertexDataPosNormalUV), BufferData.Create(DataHelper.Cube));
        }

        public static MeshData DebugCube()
        {
            return new MeshData<float>(typeof(VertexDataPosNormalUV), BufferData.Create(DataHelper.DebugCube));
        }

        public static MeshData Quad()
        {
            return new MeshData<float>(typeof(VertexDataPos2UV), BufferData.Create(DataHelper.Quad));
        }

        public static MeshData Sphere(int divisions)
        {
            var ico = new Objects.Util.IcoSphere.IcoSphereMesh(2);
            return new MeshData<VertexDataPosNormalUV>(BufferData.Create(ico.Vertices), BufferData.Create(ico.Indicies));
        }

        public static MeshData CrossLine()
        {
            return new MeshData<float>(typeof(VertexDataPosColor), BufferData.Create(DataHelper.Cross), null, AxPrimitiveType.Lines);
        }

        public static MeshData Grid(int size, bool center)
        {
            var vertices = new List<VertexDataPosColor>();

            var color = new Vector4(0.45f, 0.45f, 0.0f, 1.0f);

            int start;
            int end;
            float startPos;
            float endPos;
            if (center)
            {
                start = -size;
                end = size;
                startPos = -size;
                endPos = size;
            }
            else
            {
                start = 0;
                end = size;
                startPos = 0f;
                endPos = size;
            }

            for (var i = start; i <= end; i++)
            {
                vertices.Add(new Vector3(startPos, i, 0), color);
                vertices.Add(new Vector3(endPos, i, 0), color);

                vertices.Add(new Vector3(i, startPos, 0), color);
                vertices.Add(new Vector3(i, endPos, 0), color);
            }

            return new MeshData<VertexDataPosColor>(BufferData.Create(vertices.ToArray()), null, AxPrimitiveType.Lines);
        }

        public static MeshData Line(Vector3 start, Vector3 end, Vector4 colorStart, Vector4 colorEnd)
        {
            var vertices = new List<VertexDataPosColor>();

            vertices.Add(start, colorStart);
            vertices.Add(end, colorEnd);

            return new MeshData<VertexDataPosColor>(BufferData.Create(vertices.ToArray()), null, AxPrimitiveType.Lines);
        }

    }

}
