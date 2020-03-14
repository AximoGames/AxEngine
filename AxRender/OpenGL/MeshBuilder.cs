// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Render
{

    public static class MeshDataBuilder
    {
        public static MeshData Cube()
        {
            return new MeshData<float>(typeof(VertexDataPosNormalUV), DataHelper.Cube);
        }

        public static MeshData DebugCube()
        {
            return new MeshData<float>(typeof(VertexDataPosNormalUV), DataHelper.DebugCube);
        }

        public static MeshData Sphere(int divisions)
        {
            var ico = new AxRender.Objects.Util.IcoSphere.IcoSphereMesh(2);
            return new MeshData<VertexDataPosNormalUV>(ico.Vertices, ico.Indicies);
        }
    }

}
