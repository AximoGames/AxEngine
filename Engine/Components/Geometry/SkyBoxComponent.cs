// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using Aximo.Render;
using OpenToolkit;

namespace Aximo.Engine
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
