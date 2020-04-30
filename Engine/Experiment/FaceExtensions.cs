using System.Collections.Generic;
using System.Linq;

namespace Aximo.Engine.Mesh2
{
    public static class FaceExtensions
    {
        public static IEnumerable<int> ToIndiciesList(this IList<MeshFace> faces)
        {
            return faces.SelectMany(face => face.Indicies);
        }
    }

}
