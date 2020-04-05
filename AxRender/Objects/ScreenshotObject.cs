// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
{

    public class ScreenshotObject : RenderObjectBase
    {

        public ScreenshotObject(BufferData2D<int> data)
        {
            Data = data;
        }

        public override void Init()
        {
        }

        private BufferData2D<int> Data;

        public override void OnWorldRendered()
        {
            if (Data == null)
                Data = new BufferData2D<int>(Context.ScreenSize.X, Context.ScreenSize.Y);
            //FrameBuffer.Default.GetData(Data);

            var fb = Context.GetPipeline<ForwardRenderPipeline>().FrameBuffer;
            var txt = fb.DestinationTextures[0];
            Data.PixelFormat = txt.Format.ToGamePixelFormat();
            txt.GetTexture(Data);

            // var bmpt = Data.CreateBitmap();
            // bmpt.Save("/tmp/blubb.png");
        }

        public override void Free()
        {
        }
    }

}
