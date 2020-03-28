// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading;

using Aximo.Render;

namespace Aximo.Engine
{

    public class BufferComponent : ActorComponent
    {

        // TODO: 2D-Array of Struct
        public Bitmap Image { get; private set; }

        public void SetSize(int width, int height)
        {
            Image?.Dispose();
            Image = new Bitmap(width, height);
        }

        public void SetImage(Bitmap image)
        {
            Image = image;
        }

        private ScreenshotObject RenderObject;

        internal override void SyncChanges()
        {
            bool created = false;
            if (RenderObject == null)
            {
                created = true;
                RenderObject = new ScreenshotObject();
            }

            if (created)
                RenderContext.Current.AddObject(RenderObject);
        }

    }

}