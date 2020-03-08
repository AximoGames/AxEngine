using OpenToolkit.Mathematics;
using OpenToolkit.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace AxEngine
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
