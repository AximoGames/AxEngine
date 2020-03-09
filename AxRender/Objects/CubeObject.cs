// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace Aximo.Render
{

    public class CubeObject : SimpleVertexObject
    {

        public override void Init() {
            SetVertices(DataHelper.Cube);

            base.Init();
        }

    }

}