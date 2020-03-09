using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace Aximo.Render
{
    public class ScreenSceneObject : ScreenTextureObject
    {
        public override void Init()
        {
            SourceTexture = Context.GetPipeline<ForwardRenderPipeline>().FrameBuffer.GetDestinationTexture();
            base.Init();
        }
    }

}
