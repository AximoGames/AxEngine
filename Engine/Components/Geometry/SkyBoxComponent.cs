// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render;
using Aximo.Render.Objects;

namespace Aximo.Engine.Components.Geometry
{
    public class SkyBoxComponent : MeshComponent
    {
        public SkyBoxComponent()
        {
        }

        public SkyBoxComponent(string skyBoxPath)
        {
        }

        internal override void SyncChanges()
        {
            if (!HasChanges)
                return;

            base.SyncChanges();

            bool created = false;
            if (RenderableObject == null)
            {
                created = true;
                RenderableObject = new SkyboxObject();
            }

            var obj = (SkyboxObject)RenderableObject;

            if (created)
                RenderContext.Current.AddObject(RenderableObject);
        }

        internal override void DoDeallocation()
        {
            if (!HasDeallocation)
                return;

            if (RenderableObject == null)
                return;

            RenderableObject.Orphaned = true;
            RenderableObject = null;

            base.DoDeallocation();
        }
    }
}
