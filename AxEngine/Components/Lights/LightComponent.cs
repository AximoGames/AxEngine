// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

using Aximo.Render;

namespace Aximo.Engine
{

    public abstract class LightComponent : SceneComponent
    {
        internal ILightObject LightObject;

        protected abstract LightType LightType { get; }

        private static int ShadowIdx;

        internal override void SyncChanges()
        {
            if (!HasChanges)
                return;

            if (LightObject == null)
            {
                LightObject = new LightObject();
                LightObject.LightType = LightType;
                LightObject.ShadowTextureIndex = ShadowIdx++;
                LightObject.Name = Name;
                RenderContext.Current.AddObject(LightObject);
            }

            if (TransformChanged)
            {
                LightObject.Position = LocalToWorld().ExtractTranslation();
                TransformChanged = false;
            }

            base.SyncChanges();
        }
    }

}
