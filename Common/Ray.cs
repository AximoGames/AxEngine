﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo
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

        public static Ray FromPoints(Vector3 origin, Vector3 destination)
        {
            return new Ray(origin, destination - origin);
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
            return _Origin + (_Direction * distance);
        }

        public override string ToString()
        {
            return $"[{_Origin}, {_Direction}]";
        }
    }
}
