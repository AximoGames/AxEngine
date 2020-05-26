// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Aximo.VertexData
{
    public interface IVertexPos2UV : IVertexPosition2, IVertexUV
    {
        void Set(IVertexPos2UV source);
        void Set(VertexDataPos2UV source);

        new VertexDataPos2UV Clone();
    }
}
