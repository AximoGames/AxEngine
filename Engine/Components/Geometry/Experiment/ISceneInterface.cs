// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Components.Lights;

namespace Aximo.Engine
{
    public interface ISceneInterface
    {
        void AddLight(LightComponent light);
        void RemoveLight(LightComponent light);

        void AddPrimitive(PrimitiveComponent primitive);
        void RemovePrimitive(PrimitiveComponent primitive);
    }
}
