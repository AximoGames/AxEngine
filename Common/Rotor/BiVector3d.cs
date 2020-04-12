// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

// Original Author: https://github.com/NogginBops
// Original Source: https://github.com/opentk/opentk/pull/938

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable SA1614 // Element parameter documentation should have text
#pragma warning disable CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
#pragma warning disable SA1606 // Element documentation should have summary text
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable SA1616 // Element return value documentation should have text
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

namespace OpenToolkit.Mathematics
{
    /// <summary>
    /// A three dimentional bivector i.e. a plane at origin.
    /// Could also be though of as the normal to a plane.
    /// </summary>
    public struct BiVector3d : IEquatable<BiVector3d>
    {
        /// <summary>
        ///
        /// </summary>
        public static readonly BiVector3d UnitYZ = new BiVector3d(1, 0, 0);

        /// <summary>
        ///
        /// </summary>
        public static readonly BiVector3d UnitZX = new BiVector3d(0, 1, 0);

        /// <summary>
        ///
        /// </summary>
        public static readonly BiVector3d UnitXY = new BiVector3d(0, 0, 1);

        /// <summary>
        /// The not-X basis.
        /// </summary>
        public float NotX;

        /// <summary>
        /// The not-Y basis.
        /// </summary>
        public float NotY;

        /// <summary>
        /// The not-Z basis.
        /// </summary>
        public float NotZ;

        /// <summary>
        ///
        /// </summary>
        /// <param name="v"></param>
        public BiVector3d(Vector3 v)
        {
            NotX = v.X;
            NotY = v.Y;
            NotZ = v.Z;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        public BiVector3d(float nx, float ny, float nz)
        {
            NotX = nx;
            NotY = ny;
            NotZ = nz;
        }

        /// <summary>
        /// Gets the square magnitude (length) of this bivector.
        /// </summary>
        public float MagnitudeSquared => (NotX * NotX) + (NotY * NotY) + (NotZ * NotZ);

        /// <summary>
        /// Gets the magnitude (length) of this bivector.
        /// </summary>
        public float Magnitude => (float)Math.Sqrt(MagnitudeSquared);

        /// <summary>
        /// Functionally the same as dot product in more 'conventional' algebra.
        /// </summary>
        /// <param name="bv"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        /// <remarks>Returns a AntiScalar3D which is a one component vector (i.e a float) that flips sign when reflected.</remarks>
        public static float Wedge(BiVector3d bv, Vector3 v)
        {
            return (bv.NotX * v.X) + (bv.NotY * v.Y) + (bv.NotZ * v.Z);
        }

        /// <summary>
        /// Functionally the same as dot product in more 'conventional' algebra.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="bv"></param>
        /// <returns></returns>
        /// <remarks>Returns a scalar which is a one component vector (i.e a float) that retains sign on reflection.</remarks>
        public static float AntiWedge(in Vector3 v, in BiVector3d bv)
        {
            return (v.X * bv.NotX) + (v.Y * bv.NotY) + (v.Z * bv.NotZ);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="bv"></param>
        public static void Normalize(ref BiVector3d bv) => Normalize(bv, out bv);

        /// <summary>
        ///
        /// </summary>
        /// <param name="bv"></param>
        /// <param name="result"></param>
        public static void Normalize(in BiVector3d bv, out BiVector3d result)
        {
            float mag = bv.Magnitude;
            result.NotX = bv.NotX / mag;
            result.NotY = bv.NotY / mag;
            result.NotZ = bv.NotZ / mag;
        }

        /// <summary>
        ///
        /// </summary>
        public void Normalize() => Normalize(ref this);

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public BiVector3d Normalized()
        {
            Normalize(this, out BiVector3d bv);
            return bv;
        }

        /// <inheritdoc/>
        public bool Equals(BiVector3d other)
        {
            return NotX == other.NotX && NotY == other.NotY && NotZ == other.NotZ;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(in BiVector3d a, in BiVector3d b) => a.Equals(b);

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(in BiVector3d a, in BiVector3d b) => !a.Equals(b);

        /// <summary>
        ///
        /// </summary>
        /// <param name="v"></param>
        [Pure]
        public static explicit operator BiVector3d(in Vector3 v) => new BiVector3d(v);
    }
}
