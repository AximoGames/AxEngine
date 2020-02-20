using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{

    public interface IRenderPipeline
    {
        void Init();
        void Render(RenderContext context, Camera camera);
        IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera);
    }

    public abstract class RenderPipeline : IRenderPipeline
    {
        public virtual void Init() { }
        public abstract void Render(RenderContext context, Camera camera);
        protected virtual void Render(RenderContext context, Camera camera, IRenderableObject obj)
        {
            ObjectManager.PushDebugGroup("OnRender", obj);
            obj.OnRender();
            ObjectManager.PopDebugGroup();
        }
        public virtual IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera)
        {
            foreach (var obj in context.RenderableObjects)
                if (obj.Enabled)
                    if (obj.RenderPipelines.Contains(this))
                        yield return obj;
        }
    }

    public class PointShadowRenderPipeline : RenderPipeline
    {

        public FrameBuffer FrameBuffer { get; private set; }

        public override void Init()
        {
            FrameBuffer = new FrameBuffer(1024, 1024);
            FrameBuffer.InitCubeDepth();
        }

        public override void Render(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, FrameBuffer.Width, FrameBuffer.Height);
            FrameBuffer.Use();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

        public override IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera)
        {
            foreach (var obj in base.GetRenderObjects(context, camera))
                if (obj is IShadowObject m)
                    if (m.RenderShadow)
                        yield return m;
        }

    }

    public class DirectionalShadowRenderPipeline : RenderPipeline
    {

        public FrameBuffer FrameBuffer { get; private set; }

        public override void Init()
        {
            FrameBuffer = new FrameBuffer(1024, 1024);
            FrameBuffer.InitDepth();
        }

        public override void Render(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, FrameBuffer.Width, FrameBuffer.Height);
            FrameBuffer.Use();
            GL.Clear(ClearBufferMask.DepthBufferBit);

            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

        public override IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera)
        {
            foreach (var obj in base.GetRenderObjects(context, camera))
                if (obj is IShadowObject m)
                    if (m.RenderShadow)
                        yield return m;
        }

    }

    public class ForwardRenderPipeline : RenderPipeline
    {

        public FrameBuffer FrameBuffer;

        public override void Init()
        {
            FrameBuffer = new FrameBuffer(RenderContext.Current.ScreenWidth, RenderContext.Current.ScreenHeight);
            FrameBuffer.InitNormal();
            FrameBuffer.CreateRenderBuffer(RenderbufferStorage.Depth24Stencil8, FramebufferAttachment.DepthStencilAttachment);
        }

        public override void Render(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, context.ScreenWidth, context.ScreenHeight);
            FrameBuffer.Use();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

    }

    public class DeferredRenderPipeline : RenderPipeline
    {

        public FrameBuffer gBuffer;
        public Texture gPosition;
        public Texture gNormal;
        public Texture gAlbedoSpec;

        public override void Init()
        {
            var width = RenderContext.Current.ScreenWidth;
            var height = RenderContext.Current.ScreenHeight;

            gBuffer = new FrameBuffer(width, height);
            gBuffer.InitNormal();

            gBuffer.ObjectLabel = nameof(gBuffer);

            gPosition = new Texture(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            gPosition.ObjectLabel = nameof(gPosition);
            gPosition.SetNearestFilter();
            gBuffer.BindTexture(gPosition, FramebufferAttachment.ColorAttachment0);

            gNormal = new Texture(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            gNormal.ObjectLabel = nameof(gNormal);
            gNormal.SetNearestFilter();
            gBuffer.BindTexture(gNormal, FramebufferAttachment.ColorAttachment1);

            gAlbedoSpec = new Texture(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            gAlbedoSpec.SetNearestFilter();
            gAlbedoSpec.ObjectLabel = nameof(gAlbedoSpec);
            gBuffer.BindTexture(gAlbedoSpec, FramebufferAttachment.ColorAttachment2);

            GL.DrawBuffers(3, new DrawBuffersEnum[] {
                DrawBuffersEnum.ColorAttachment0,
                DrawBuffersEnum.ColorAttachment1,
                DrawBuffersEnum.ColorAttachment2 });

            var rboDepth = new RenderBuffer(gBuffer, RenderbufferStorage.DepthComponent, FramebufferAttachment.DepthAttachment);
            rboDepth.ObjectLabel = nameof(rboDepth);

            gBuffer.Check();
        }

        public DeferredPass Pass;

        public enum DeferredPass
        {
            Pass1,
            Pass2,
        }

        public override void Render(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, context.ScreenWidth, context.ScreenHeight);
            gBuffer.Use();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Pass = DeferredPass.Pass1;
            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);

            Pass = DeferredPass.Pass2;
            context.GetPipeline<ForwardRenderPipeline>().FrameBuffer.Use();
            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

    }

    public class ScreenPipeline : RenderPipeline
    {


        public override void Init()
        {
        }

        public override void Render(RenderContext context, Camera camera)
        {
            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

    }

}