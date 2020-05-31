// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;
using OpenToolkit.Mathematics;

namespace Aximo.Engine.Components.Lights
{
    public class PointLightComponent : LightComponent
    {
        protected override LightType LightType => LightType.Point;
    }
}
