// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render.Pipelines;

namespace Aximo.Render.Objects
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
                Data = new BufferData2D<int>(Context.ScreenPixelSize.X, Context.ScreenPixelSize.Y);
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
