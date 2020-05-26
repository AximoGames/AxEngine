// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Aximo.VertexData;

namespace Aximo
{
    public static class FaceExtensions
    {
        public static IEnumerable<int> ToIndiciesList<T>(this IList<MeshFace<T>> faces)
            where T : IVertex
        {
            // TODO: Improve performance
            return faces.SelectMany(face => face.GetIndicies());
        }
    }
}
