// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo.Engine.Components.Lights
{
    public class PointLightComponent : LightComponent
    {
        protected override LightType LightType => LightType.Point;

        private float _Linear = 0.1f;
        public float Linear
        {
            get => _Linear;
            set { if (_Linear == value) return; _Linear = value; LightAttributesChanged(); }
        }

        private float _Quadric = 0.0f;
        public float Quadric
        {
            get => _Quadric;
            set { if (_Quadric == value) return; _Quadric = value; LightAttributesChanged(); }
        }
    }
}
