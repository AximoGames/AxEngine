// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
{
    public class ScreenPipeline : RenderPipeline
    {

        public override void Init()
        {
        }

        public override void Render(RenderContext context, Camera camera)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Disable(EnableCap.DepthTest);
            GL.ClearColor(1.0f, 0.0f, 1.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

    }

}