// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo
{
    public static class VectorHelper
    {
        /// <summary>
        /// The wedge product of two vectors.
        /// This creates a bivector that represents the plane formed by these vectors.
        /// </summary>
        /// <param name="v1">The first vector.</param>
        /// <param name="v2">The second vector.</param>
        /// <returns>The resulting bivector.</returns>
        public static BiVector3d Wedge(Vector3 v1, Vector3 v2)
        {
            Wedge(v1, v2, out BiVector3d bv);
            return bv;
        }

        /// <summary>
        /// The wedge product of two vectors.
        /// This creates a bivector that represents the plane formed by these vectors.
        /// </summary>
        public static void Wedge(in Vector3 v1, in Vector3 v2, out BiVector3d bv)
        {
            bv.b01 = (v1.X * v2.Y) - (v1.Y * v2.X);
            bv.b02 = (v1.X * v2.Z) - (v1.Z * v2.X);
            bv.b12 = (v1.Y * v2.Z) - (v1.Z * v2.Y);
        }

        public static void Wedge(in Vector4 p, in Vector4 q, out BiVector4d bv)
        {
            bv.WX = q.X - p.X;
            bv.WY = q.Y - p.Y;
            bv.WZ = q.Z - p.Z;
            bv.YZ = (p.Y * q.Z) - (p.Z * q.Y);
            bv.ZX = (p.Z * q.X) - (p.X * q.Z);
            bv.XY = (p.X * q.Y) - (p.Y * q.Y);
        }

        public static void AntiWedge(in Vector4 v, in AntiVector4d tv, out float s)
        {
            s = (v.X * tv.NotX) + (v.Y * tv.NotY) + (v.Z * tv.NotZ);
        }
    }
}
