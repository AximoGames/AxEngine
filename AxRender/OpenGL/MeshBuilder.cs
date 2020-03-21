// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Render
{

    public static class MeshBuilder
    {
        public static StaticMesh Cube()
        {
            return new StaticMesh(MeshDataBuilder.Cube(), Material.GetDefault());
        }

        public static StaticMesh DebugCube()
        {
            return new StaticMesh(MeshDataBuilder.DebugCube(), Material.GetDefault());
        }

        public static StaticMesh Sphere(int divisions)
        {
            return new StaticMesh(MeshDataBuilder.Sphere(divisions), Material.GetDefault());
        }
    }

}
