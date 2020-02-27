using System;
using OpenTK;

namespace AxEngine
{

    public struct Ray
    {
        private Vector3 _Origin;
        private Vector3 _Direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            _Origin = origin;
            _Direction = direction.Normalized();
        }

        public Vector3 Origin
        {
            get { return _Origin; }
            set { _Origin = value; }
        }

        public Vector3 Direction
        {
            get { return _Direction; }
            set { _Direction = value.Normalized(); }
        }

        /// <summary>
        /// Returns a point at 'distance' units along the ray.
        /// <summary>
        public Vector3 GetPoint(float distance)
        {
            return _Origin + _Direction * distance;
        }

        public override string ToString()
        {
            return $"[{_Origin}, {_Direction}]";
        }

    }
}