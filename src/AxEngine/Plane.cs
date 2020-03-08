using System;
using OpenToolkit.Mathematics;

namespace AxEngine
{
    public struct Plane
    {
        Vector3 _Normal;
        float _Distance;

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

        public override string ToString()
        {
            return $"[{_Normal}, {_Distance}]";
        }

    }
}