// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Aximo.Render;

namespace Aximo.Engine
{
    // public class GameStaticMesh : GameMesh
    // {
    //     public GameStaticMesh() : base() { }
    //     public GameStaticMesh(MeshData meshData) : base(meshData) { }
    //     public GameStaticMesh(MeshData meshData, GameMaterial material) : base(meshData, material) { }
    // }

    // public class GameDynamicMesh : GameMesh
    // {
    //     public GameDynamicMesh() : base() { }
    //     public GameDynamicMesh(MeshData meshData) : base(meshData) { }
    //     public GameDynamicMesh(MeshData meshData, GameMaterial material) : base(meshData, material) { }
    // }

    // public abstract class GameMesh
    // {

    //     public GameMesh() { }
    //     public GameMesh(MeshData meshData)
    //     {
    //         MeshData = meshData;
    //     }

    //     public GameMesh(MeshData meshData, GameMaterial material)
    //     {
    //         MeshData = meshData;
    //         Materials.Add(material);
    //     }

    //     public GameMaterial Material
    //     {
    //         get
    //         {
    //             if (Materials.Count == 0)
    //                 return null;
    //             return Materials[0];
    //         }
    //         set
    //         {
    //             if (Materials.Count == 0)
    //                 Materials.Add(value);
    //             else
    //                 Materials[0] = value;
    //         }
    //     }

    //     public List<GameMaterial> Materials = new List<GameMaterial>();
    //     public MeshData MeshData { get; private set; }

    //     public int VertexCount => MeshData?.VertexCount ?? 0;
    // }
}
