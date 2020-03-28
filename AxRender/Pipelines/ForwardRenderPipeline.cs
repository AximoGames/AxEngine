// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenTK.Graphics.OpenGL4;

namespace Aximo.Render
{
    public class ForwardRenderPipeline : RenderPipeline
    {

        public FrameBuffer FrameBuffer;

        private void CreateFrameBuffer()
        {
            FrameBuffer = new FrameBuffer(RenderContext.Current.ScreenSize.X, RenderContext.Current.ScreenSize.Y)
            {
                ObjectLabel = "Forward",
            };
            FrameBuffer.InitNormal();
            //FrameBuffer.CreateRenderBuffer(RenderbufferStorage.Depth24Stencil8, FramebufferAttachment.DepthStencilAttachment);
            FrameBuffer.CreateRenderBuffer(RenderbufferStorage.DepthComponent32f, FramebufferAttachment.DepthAttachment);
        }

        public override void BeforeInit()
        {
            CreateFrameBuffer();
        }

        public override void Init()
        {
        }

        public override void InitRender(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, context.ScreenSize.X, context.ScreenSize.Y);
            FrameBuffer.Bind();

            var bgColor = context.BackgroundColor;
            GL.ClearColor(context.BackgroundColor.X, bgColor.Y, bgColor.Z, bgColor.W);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public override void Render(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, context.ScreenSize.X, context.ScreenSize.Y);
            FrameBuffer.Bind();
            GL.Enable(EnableCap.DepthTest);

            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

        public override void OnScreenResize()
        {
            // if (RenderContext.Current.ScreenSize.X == 100)
            //     throw new Exception();
            FrameBuffer.Resize(RenderContext.Current.ScreenSize.X, RenderContext.Current.ScreenSize.Y);
        }

    }

}