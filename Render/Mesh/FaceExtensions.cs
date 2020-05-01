using System.Collections.Generic;
using System.Linq;
using Aximo.Render;

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
