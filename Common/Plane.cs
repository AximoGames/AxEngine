// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit.Mathematics;

namespace Aximo
{

    public struct Plane
    {
        private Vector3 _Normal;
        private float _Distance;

        public float A
        {
            get => _Normal.X;
            set => _Normal.X = value;
        }

        public float B
        {
            get => _Normal.Y;
            set => _Normal.Y = value;
        }

        public float C
        {
            get => _Normal.Z;
            set => _Normal.Z = value;
        }

        public float D
        {
            get => _Distance;
            set => _Distance = value;
        }

        /// <summary>
        /// Normal vector of the plane.
        /// </summary>
        public Vector3 Normal
        {
            get { return _Normal; }
            set { _Normal = value; }
        }

        /// <summary>
        /// Distance from the origin to the plane.
        /// </summary>
        public float Distance
        {
            get { return _Distance; }
            set { _Distance = value; }
        }

        public Plane(Vector3 normal, Vector3 point)
        {
            _Normal = Vector3.Normalize(normal);
            _Distance = -Vector3.Dot(_Normal, point);
        }

        public Plane(Vector3 normal, float distance)
        {
            _Normal = Vector3.Normalize(normal);
            _Distance = distance;
        }

        /// <summary>
        /// Creates a plane.
        /// </summary>
        public Plane(Vector3 a, Vector3 b, Vector3 c)
        {
            _Normal = Vector3.Normalize(Vector3.Cross(b - a, c - a));
            _Distance = -Vector3.Dot(_Normal, a);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plane"/> struct.
        /// </summary>
        /// <param name="a">The X component of the normal.</param>
        /// <param name="b">The Y component of the normal.</param>
        /// <param name="c">The Z component of the normal.</param>
        /// <param name="d">The distance of the plane along its normal from the origin.</param>
        public Plane(float a, float b, float c, float d)
        {
            _Normal.X = a;
            _Normal.Y = b;
            _Normal.Z = c;
            _Distance = d;
        }

        /// <summary>
        /// Make the plane face the opposite direction
        /// </summary>
        public Plane Flip()
        {
            return new Plane(-_Normal, -_Distance);
        }

        /// <summary>
        /// Creates a plane that's translated into a given direction
        /// </summary>
        public Plane Translate(Vector3 translation)
        {
            return new Plane(_Normal, _Distance += Vector3.Dot(_Normal, translation));
        }

        /// <summary
        /// Calculates the closest point on the plane.
        /// </summary>
        public Vector3 ClosestPointOnPlane(Vector3 point)
        {
            var pointToPlaneDistance = Vector3.Dot(_Normal, point) + _Distance;
            return point - (_Normal * pointToPlaneDistance);
        }

        /// <summary
        /// Returns a signed distance from plane to point.
        /// </summary>
        public float GetDistanceToPoint(Vector3 point)
        {
            return Vector3.Dot(_Normal, point) + _Distance;
        }

        /// <summary>
        /// Is a point on the positive side of the plane?
        /// </summary>
        public bool IsPositiveSide(Vector3 point)
        {
            return Vector3.Dot(_Normal, point) + _Distance > 0.0F;
        }

        /// <summary>
        /// Are two points on the same side of the plane?
        /// </summary>
        public bool SameSide(Vector3 point1, Vector3 point2)
        {
            float dist0 = GetDistanceToPoint(point1);
            float dist1 = GetDistanceToPoint(point2);
            return (dist0 > 0.0f && dist1 > 0.0f)
                || (dist0 <= 0.0f && dist1 <= 0.0f);
        }

        /// <summary>
        /// Intersects a ray with the plane
        /// <summary>.
        public bool Raycast(Ray ray, out float enter)
        {
            float dirDot = Vector3.Dot(ray.Direction, _Normal);
            float oriDot = -Vector3.Dot(ray.Origin, _Normal) - _Distance;

            if (AxMath.Approximately(dirDot, 0.0f))
            {
                enter = 0.0f;
                return false;
            }

            enter = oriDot / dirDot;

            return enter > 0.0f;
        }

        public void Normalize()
        {
            float mag = MathF.Sqrt((A * A) + (B * B) + (C * C));

            A /= mag;
            B /= mag;
            C /= mag;
            D /= mag;
        }

        public bool IsOutside(Box3 treeAABB)
        {
            //return TestAABBPlane(treeAABB, this);
            Vector3 axisVert;

            // x-axis
            if (_Normal.X < 0.0f)    // Which AABB vertex is furthest down (plane normals direction) the x axis
                axisVert.X = treeAABB.Min.X;
            else
                axisVert.X = treeAABB.Max.X;

            // y-axis
            if (_Normal.Y < 0.0f)    // Which AABB vertex is furthest down (plane normals direction) the y axis
                axisVert.Y = treeAABB.Min.Y;
            else
                axisVert.Y = treeAABB.Max.Y;

            // z-axis
            if (_Normal.Z < 0.0f)    // Which AABB vertex is furthest down (plane normals direction) the z axis
                axisVert.Z = treeAABB.Min.Z;
            else
                axisVert.Z = treeAABB.Max.Z;

            // Now we get the signed distance from the AABB vertex that's furthest down the frustum planes normal,
            // and if the signed distance is negative, then the entire bounding box is behind the frustum plane, which means
            // that it should be culled
            if (Vector3.Dot(_Normal, axisVert) + _Distance < 0.0f)
                return true;

            return false;
        }

        // Test if AABB b intersects plane p
        public bool TestAABBPlane(Box3 b, Plane p)
        {
            // TODO: TEST!

            // Convert AABB to center-extents representation
            Vector3 c = (b.Max + b.Min) * 0.5f; // Compute AABB center
            Vector3 e = b.Max - c; // Compute positive extents

            // Compute the projection interval radius of b onto L(t) = b.c + t * p.n
            float r = (e[0] * Math.Abs(p.Normal.X)) + (e[1] * Math.Abs(p.Normal.Y)) + (e[2] * Math.Abs(p.Normal.Z));

            // Compute distance of box center from plane
            float s = Vector3.Dot(p.Normal, c) - p.Distance;

            // Intersection occurs when distance s falls within [-r,+r] interval
            return Math.Abs(s) <= r;
        }

        public override string ToString()
        {
            return $"[{_Normal}, {_Distance}]";
        }


    }
}
