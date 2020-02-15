using OpenTK;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace ProcEngine
{

    public interface IRenderPipeline
    {
        void Render(RenderContext context, Camera camera);
    }

    public abstract class RenderPipeline : IRenderPipeline
    {
        public virtual void Init() { }
        public abstract void Render(RenderContext context, Camera camera);
    }

    public class PointShadowRenderPipeline : RenderPipeline
    {

        public FrameBuffer ShadowCubeFb;

        public override void Init()
        {
            ShadowCubeFb = new FrameBuffer(1024, 1024);
            ShadowCubeFb.InitCubeDepth();
        }

        public override void Render(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, ShadowCubeFb.Width, ShadowCubeFb.Height);
            ShadowCubeFb.Use();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            foreach (var obj in context.ShadowObjects)
                if (obj.Enabled && obj.RenderShadow)
                    obj.OnRenderCubeShadow();
        }

    }

    public class DirectionalShadowRenderPipeline : RenderPipeline
    {

        public FrameBuffer ShadowFb;

        public override void Init()
        {
            ShadowFb = new FrameBuffer(1024, 1024);
            ShadowFb.InitDepth();
        }

        public override void Render(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, ShadowFb.Width, ShadowFb.Height);
            ShadowFb.Use();
            GL.Clear(ClearBufferMask.DepthBufferBit);

            foreach (var obj in context.ShadowObjects)
                if (obj.Enabled && obj.RenderShadow)
                    obj.OnRenderShadow();
        }

    }

    public class ForwardRenderPipeline : RenderPipeline
    {

        public override void Init()
        {
        }

        public override void Render(RenderContext context, Camera camera)
        {
        }

    }

    public class DeferredRenderPipeline : RenderPipeline
    {
        public override void Init()
        {
        }

        public override void Render(RenderContext context, Camera camera)
        {
        }
    }

}