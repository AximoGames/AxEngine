// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.VertexData
{
    public interface IVertexPosNormalColor : IVertexPosition3, IVertexNormal, IVertexColor
    {
        void Set(IVertexPosNormalColor source);
        void Set(VertexDataPosNormalColor source);

        new VertexDataPosNormalColor Clone();
    }
}
