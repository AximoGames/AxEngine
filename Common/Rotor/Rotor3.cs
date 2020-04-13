// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Aximo;

// Original Author: https://github.com/NogginBops
// Original Source: https://github.com/opentk/opentk/pull/938

#pragma warning disable SA1614 // Element parameter documentation should have text
#pragma warning disable CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
#pragma warning disable SA1606 // Element documentation should have summary text
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable SA1616 // Element return value documentation should have text
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()

namespace OpenToolkit.Mathematics
{
    /// <summary>
    ///
    /// </summary>
    public struct Rotor3 : IEquatable<Rotor3>
    {

        public static readonly Rotor3 Identity = new Rotor3(1, 0, 0, 0);

        // FIXME: Better name!
        public float A;

        public float b12;
        public float b02;
        public float b01;

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="yz"></param>
        /// <param name="zx"></param>
        /// <param name="xy"></param>
        public Rotor3(float a, float xy, float xz, float yz)
        {
            A = a;
            b01 = xy;
            b02 = xz;
            b12 = yz;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="bv"></param>
        public Rotor3(float a, in BiVector3d bv)
        {
            A = a;
            b01 = bv.b01;
            b02 = bv.b02;
            b12 = bv.b12;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public Rotor3(Vector3 from, Vector3 to)
        {
            //// Too expensive?
            //from.Normalize();
            //to.Normalize();

            A = 1 + Vector3.Dot(to, from);
            BiVector3d bivec = VectorHelper.Wedge(to, from);
            b01 = bivec.b01;
            b02 = bivec.b02;
            b12 = bivec.b12;

            Normalize(this, out this);
        }

        /// <summary>
        ///
        /// </summary>
        public float Length => (float)Math.Sqrt(LengthSquared);

        /// <summary>
        ///
        /// </summary>
        public float LengthSquared => (A * A) + (b01 * b01) + (b02 * b02) + (b12 * b12);

        public BiVector3d BiVector => new BiVector3d(b12, b02, b01);

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Rotor3 Multiply(in Rotor3 a, in Rotor3 b)
        {
            Multiply(a, b, out Rotor3 res);
            return res;
        }

        public static void Multiply(in Rotor3 a, in Rotor3 b, out Rotor3 res)
        {
            res.A = (a.A * b.A) - (a.b12 * b.b12) - (a.b02 * b.b02) - (a.b01 * b.b01);
            res.b12 = (a.b12 * b.A) + (a.A * b.b12) + (a.b01 * b.b02) - (a.b02 * b.b01);
            res.b02 = (a.b02 * b.A) + (a.A * b.b02) - (a.b01 * b.b12) + (a.b12 * b.b01);
            res.b01 = (a.b01 * b.A) + (a.A * b.b01) + (a.b02 * b.b12) - (a.b12 * b.b02);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="r"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3 Rotate(in Rotor3 r, in Vector3 v)
        {
            Rotate(r, v, out Vector3 res);
            return res;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p"></param>
        /// <param name="x"></param>
        /// <param name="r"></param>
        public static void Rotate(in Rotor3 p, in Vector3 x, out Vector3 r)
        {
            x.Normalize();

            Vector3 q = Vector3.Zero;
            q[0] = p.A * x[0] + x[1] * p.b01 + x[2] * p.b02;
            q[1] = p.A * x[1] - x[0] * p.b01 + x[2] * p.b12;
            q[2] = p.A * x[2] - x[0] * p.b02 - x[1] * p.b12;

            float q012 = -x[0] * p.b12 + x[1] * p.b02 - x[2] * p.b01; // trivector

            r = Vector3.Zero;
            r[0] = p.A * q[0] + q[1] * p.b01 + q[2] * p.b02 - q012 * p.b12;
            r[1] = p.A * q[1] - q[0] * p.b01 + q012 * p.b02 + q[2] * p.b12;
            r[2] = p.A * q[2] - q012 * p.b01 - q[0] * p.b02 - q[1] * p.b12;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static Rotor3 Conjugate(in Rotor3 r)
        {
            Conjugate(r, out Rotor3 c);
            return c;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="r"></param>
        /// <param name="c"></param>
        public static void Conjugate(in Rotor3 r, out Rotor3 c)
        {
            c = new Rotor3(r.A, -r.b01, -r.b02, -r.b12);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static Rotor3 Normalize(in Rotor3 rot)
        {
            Normalize(rot, out Rotor3 res);
            return res;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rot"></param>
        /// <param name="res"></param>
        public static void Normalize(in Rotor3 rot, out Rotor3 res)
        {
            float l = rot.Length;
            res.A = rot.A / l;
            res.b01 = rot.b01 / l;
            res.b02 = rot.b02 / l;
            res.b12 = rot.b12 / l;
        }

        public static Matrix3 ToMatrix3(in Rotor3 rot)
        {
            ToMatrix3(rot, out Matrix3 mat);
            return mat;
        }

        public static void ToMatrix3(in Rotor3 rot, out Matrix3 res)
        {
            // TODO: Optimize
            Rotate(rot, Vector3.UnitX, out Vector3 x);
            Rotate(rot, Vector3.UnitY, out Vector3 y);
            Rotate(rot, Vector3.UnitZ, out Vector3 z);
            res = new Matrix3(x.X, y.X, z.X, x.Y, y.Y, z.Y, x.Z, y.Z, z.Z);
        }

        /// <summary>
        /// Geometric product. This will create a rotor with double the angle from a to b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Rotor3 Geo(in Vector3 a, in Vector3 b)
        {
            Geo(a, b, out Rotor3 res);
            return res;
        }

        /// <summary>
        /// Geometric product. This will create a rotor with double the angle from a to b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="res"></param>
        public static void Geo(in Vector3 a, in Vector3 b, out Rotor3 res)
        {
            res = new Rotor3(Vector3.Dot(a, b), VectorHelper.Wedge(a, b));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Rotor3 other)
        {
            return A == other.A && BiVector == other.BiVector;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(in Rotor3 a, in Rotor3 b) => a.Equals(b);

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(in Rotor3 a, in Rotor3 b) => !a.Equals(b);

        /// <summary>
        ///
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Rotor3 operator *(in Rotor3 a, in Rotor3 b)
        {
            Multiply(a, b, out Rotor3 res);
            return res;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rot"></param>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 operator *(in Rotor3 rot, in Vector3 vec)
        {
            Rotate(rot, vec, out Vector3 res);
            return res;
        }
    }
}
