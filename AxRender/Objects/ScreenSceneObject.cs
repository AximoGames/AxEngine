﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

namespace Aximo.Render
{
    public class ScreenSceneObject : ScreenTextureObject
    {
        public override void Init() {
            SourceTexture = Context.GetPipeline<ForwardRenderPipeline>().FrameBuffer.GetDestinationTexture();
            base.Init();
        }
    }

}