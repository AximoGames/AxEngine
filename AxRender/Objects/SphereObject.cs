// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.Render
{
    public class SphereObject : SimpleVertexObject
    {

        public override void Init()
        {
            var ico = new Util.IcoSphere.Mesh_SphereICO(2);
            SetVertices(ico.Vertices);
            SetIndicies(ico.Indicies);

            base.Init();
        }

    }

}