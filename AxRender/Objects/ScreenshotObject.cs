// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Aximo.Render
{
    public class ScreenshotObject : GameObjectBase
    {
        public override void Init()
        {
        }

        private BufferData2<int> Data;

        public override void OnWorldRendered()
        {
            if (Data == null)
                Data = new BufferData2<int>(Context.ScreenSize.X, Context.ScreenSize.Y);
            //FrameBuffer.Default.GetData(Data);

            var fb = Context.GetPipeline<ForwardRenderPipeline>().FrameBuffer;
            var txt = fb.DestinationTextures[0];
            txt.GetTexture(Data);

            var bmpt = Data.CreateBitmap();
            bmpt.Save("/tmp/blubb.png");
        }

        public override void Free()
        {
        }
    }

}
