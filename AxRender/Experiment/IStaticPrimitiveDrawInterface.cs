// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Render
{

    public class GScene
    {

    }

    public interface IPrimitiveDrawInterface
    {
        void DrawMesh(MeshBatch mesh);
        //void DrawLine();
        //void DrawPoint();
    }

    public interface IStaticPrimitiveDrawInterface : IPrimitiveDrawInterface
    {
    }

}
