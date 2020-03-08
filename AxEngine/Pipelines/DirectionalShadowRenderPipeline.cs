using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace AxEngine
{
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
            FrameBuffer.Bind();
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

}