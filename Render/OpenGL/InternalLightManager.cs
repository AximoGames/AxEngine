// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Aximo.Render
{
    public static class InternalLightManager
    {

        private static SlotAllocator<int> PointLayer = new SlotAllocator<int>(Enumerable.Range(0, 300));
        private static SlotAllocator<int> DirectionalLayer = new SlotAllocator<int>(Enumerable.Range(0, 300));

        private static SlotAllocator<int> GetAllocator(ILightObject lightObject)
        {
            switch (lightObject.LightType)
            {
                case LightType.Point:
                    return PointLayer;
                case LightType.Directional:
                    return DirectionalLayer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lightObject.LightType));
            }
        }

        public static void RequestShadowLayer(ILightObject lightObject)
        {
            lightObject.ShadowTextureIndex = GetAllocator(lightObject).Alloc();
        }

        public static void FreeShadowLayer(ILightObject lightObject)
        {
            GetAllocator(lightObject).Free(lightObject.ShadowTextureIndex);
        }
    }
}
