// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Aximo.Render.Pipelines;

namespace Aximo.Render.Objects
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
