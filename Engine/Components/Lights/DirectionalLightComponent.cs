// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;

namespace Aximo.Engine.Components.Lights
{
    public class DirectionalLightComponent : LightComponent
    {
        protected override LightType LightType => LightType.Directional;
    }
}
