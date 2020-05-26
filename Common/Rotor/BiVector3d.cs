// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.Contracts;

// Original Author: https://github.com/NogginBops
// Original Source: https://github.com/opentk/opentk/pull/938

#pragma warning disable CA2225 // Operator overloads have named alternates
#pragma warning disable SA1614 // Element parameter documentation should have text
#pragma warning disable CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
#pragma warning disable SA1606 // Element documentation should have summary text
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable SA1616 // Element return value documentation should have text
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter

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
        public static readonly BiVector3d UnitXY = new BiVector3d(0, 0, 1);

        /// <summary>
        ///
        /// </summary>
        public static readonly BiVector3d UnitXZ = new BiVector3d(0, 1, 0);

        /// <summary>
        ///
        /// </summary>
        public static readonly BiVector3d UnitYZ = new BiVector3d(1, 0, 0);

        /// <summary>
        /// The not-X basis.
        /// </summary>
        public float b12;

        /// <summary>
        /// The not-Y basis.
        /// </summary>
        public float b02;

        /// <summary>
        /// The not-Z basis.
        /// </summary>
        public float b01;

        /// <summary>
        ///
        /// </summary>
        /// <param name="v"></param>
        public BiVector3d(Vector3 v)
        {
            b01 = v.Z;
            b02 = v.Y;
            b12 = v.X;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        public BiVector3d(float b01, float b02, float b12)
        {
            this.b01 = b01;
            this.b02 = b02;
            this.b12 = b12;
        }

        /// <summary>
        /// Gets the square magnitude (length) of this bivector.
        /// </summary>
        public float MagnitudeSquared => (b01 * b01) + (b02 * b02) + (b12 * b12);

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
            return (bv.b12 * v.X) + (bv.b02 * v.Y) + (bv.b01 * v.Z);
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
            return (v.X * bv.b12) + (v.Y * bv.b02) + (v.Z * bv.b01);
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
            result.b01 = bv.b01 / mag;
            result.b02 = bv.b02 / mag;
            result.b12 = bv.b12 / mag;
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
            return b01 == other.b01 && b02 == other.b02 && b12 == other.b12;
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
