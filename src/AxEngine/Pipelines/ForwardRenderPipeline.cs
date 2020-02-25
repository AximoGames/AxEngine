using OpenTK.Graphics.OpenGL4;

namespace ProcEngine
{
    public class ForwardRenderPipeline : RenderPipeline
    {

        public FrameBuffer FrameBuffer;

        public override void BeforeInit()
        {
            FrameBuffer = new FrameBuffer(RenderContext.Current.ScreenWidth, RenderContext.Current.ScreenHeight);
            FrameBuffer.ObjectLabel = "Forward";
            FrameBuffer.InitNormal();
            //FrameBuffer.CreateRenderBuffer(RenderbufferStorage.Depth24Stencil8, FramebufferAttachment.DepthStencilAttachment);
            FrameBuffer.CreateRenderBuffer(RenderbufferStorage.DepthComponent32f, FramebufferAttachment.DepthAttachment);
        }

        public override void Init()
        {
        }

        public override void InitRender(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, context.ScreenWidth, context.ScreenHeight);
            FrameBuffer.Use();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public override void Render(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, context.ScreenWidth, context.ScreenHeight);
            FrameBuffer.Use();
            GL.Enable(EnableCap.DepthTest);

            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

    }

}