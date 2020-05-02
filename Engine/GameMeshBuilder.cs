// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;

namespace Aximo.Engine
{
    public static class GameMeshBuilder
    {
        public static StaticMeshComponent Cube()
        {
            return new StaticMeshComponent(MeshDataBuilder.Cube(), GameMaterial.Default);
        }

        public static StaticMeshComponent DebugCube()
        {
            return new StaticMeshComponent(MeshDataBuilder.DebugCube(), GameMaterial.Default);
        }

        public static StaticMeshComponent Sphere(int divisions)
        {
            return new StaticMeshComponent(Mesh.CreateSphere(divisions), GameMaterial.Default);
        }
    }
}
