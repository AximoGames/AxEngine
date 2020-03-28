// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

        public override void OnWorldRendered()
        {
            var bmp = FrameBuffer.GetTexture(0, Context.ScreenSize.X, Context.ScreenSize.Y);
        }

        public override void Free()
        {
        }
    }

}
