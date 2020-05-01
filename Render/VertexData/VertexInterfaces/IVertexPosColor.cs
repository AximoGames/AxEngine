// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public interface IVertexPosColor : IVertexPosition3, IVertexColor
    {
        void Set(IVertexPosColor source);
        void Set(VertexDataPosColor source);
        new VertexDataPosColor Clone();
    }

}
