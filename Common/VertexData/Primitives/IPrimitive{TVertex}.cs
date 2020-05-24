// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Aximo.VertexData
{
    public interface IPrimitive<TVertex> : IPrimitive
    {
        TVertex this[int index] { get; set; }
        void CopyTo(ICollection<IPrimitive<TVertex>> destination);
    }
}
