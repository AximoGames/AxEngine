// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo.Engine.Components.Lights
{
    public class DirectionalLightComponent : LightComponent
    {
        protected override LightType LightType => LightType.Directional;

        private Vector3 _Direction = new Vector3(0, 0, -1).Normalized();
        public Vector3 Direction
        {
            get => _Direction;
            set { if (_Direction == value) return; _Direction = value; LightAttributesChanged = true; }
        }

        private bool LightAttributesChanged = true;

        internal override void SyncChanges()
        {
            if (!HasChanges)
                return;

            base.SyncChanges();

            if (LightAttributesChanged)
                LightObject.Direction = _Direction;
        }
    }
}
