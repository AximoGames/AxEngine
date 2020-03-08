using OpenToolkit.Graphics.OpenGL4;
using System;

namespace AxEngine
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