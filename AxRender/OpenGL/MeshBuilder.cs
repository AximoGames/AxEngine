// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Render
{

    public static class MeshBuilder
    {
        public static Mesh Cube()
        {
            return new Mesh(MeshDataBuilder.Cube(), Material.GetDefault());
        }

        public static Mesh DebugCube()
        {
            return new Mesh(MeshDataBuilder.DebugCube(), Material.GetDefault());
        }

        public static Mesh Sphere(int divisions)
        {
            return new Mesh(MeshDataBuilder.Sphere(divisions), Material.GetDefault());
        }
    }

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
