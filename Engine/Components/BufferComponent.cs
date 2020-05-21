// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

using Aximo.Render;
using Aximo.Render.Objects;

namespace Aximo.Engine
{
    public class BufferComponent : ActorComponent
    {
        public BufferComponent()
        {
            SetData(new BufferData2D<int>());
        }

        public BufferComponent(BufferData2D<int> bufferData)
        {
            SetData(bufferData);
        }

        public void SetData(BufferData2D<int> bufferData)
        {
            BufferData = bufferData;
        }

        public BufferData2D<int> BufferData;

        private ScreenshotObject RenderObject;

        internal override void SyncChanges()
        {
            bool created = false;
            if (RenderObject == null)
            {
                created = true;
                RenderObject = new ScreenshotObject(BufferData);
            }

            if (created)
                RenderContext.Current.AddObject(RenderObject);
        }
    }
}
