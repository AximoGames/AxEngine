using OpenTK.Graphics.OpenGL4;
using System;

namespace ProcEngine
{
    public class DeferredRenderPipeline : RenderPipeline
    {

        public FrameBuffer gBuffer;
        public Texture gPosition;
        public Texture gNormal;
        public Texture gAlbedoSpec;

        private Shader _DefLightShader;
        private float[] _vertices = DataHelper.Quad;

        private VertexArrayObject vao;
        private BufferObject vbo;

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

            _DefLightShader = new Shader("Shaders/deferred-shading.vert", "Shaders/deferred-shading.frag");
            _DefLightShader.SetInt("gPosition", 0);
            _DefLightShader.SetInt("gNormal", 1);
            _DefLightShader.SetInt("gAlbedoSpec", 2);

            vbo = new BufferObject();
            vbo.Create();
            vbo.Use();

            var layout = new VertexLayout();
            layout.AddAttribute<float>(_DefLightShader.GetAttribLocation("aPos"), 2);
            layout.AddAttribute<float>(_DefLightShader.GetAttribLocation("aTexCoords"), 2);

            vao = new VertexArrayObject(layout, vbo);
            vao.Create();

            vao.SetData(_vertices);
        }

        public DeferredPass Pass;

        public override void Render(RenderContext context, Camera camera)
        {
            ObjectManager.PushDebugGroup("OnRender Pass1", this);
            RenderPass1(context, camera);
            ObjectManager.PopDebugGroup();

            ObjectManager.PushDebugGroup("OnRender Pass2", this);
            RenderPass2(context, camera);
            ObjectManager.PopDebugGroup();
        }

        private void RenderPass1(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, context.ScreenWidth, context.ScreenHeight);
            gBuffer.Use();
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Pass = DeferredPass.Pass1;
            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);

        }

        private void RenderPass2(RenderContext context, Camera camera)
        {
            Pass = DeferredPass.Pass2;
            GL.ClearColor(0.1f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Needed?
            // context.GetPipeline<ForwardRenderPipeline>().FrameBuffer.Use();
            // foreach (var obj in GetRenderObjects(context, camera))
            //     Render(context, camera, obj);

            ObjectManager.PushDebugGroup("OnRender LightShader", this);

            _DefLightShader.Use();

            gPosition.Use(TextureUnit.Texture0);
            gNormal.Use(TextureUnit.Texture1);
            gAlbedoSpec.Use(TextureUnit.Texture2);

            _DefLightShader.SetVector3("viewPos", camera.Position);

            context.GetPipeline<ForwardRenderPipeline>().FrameBuffer.Use();
            vao.Use();
            vao.Draw();

            ObjectManager.PopDebugGroup();
        }
    }

    public enum DeferredPass
    {
        Pass1,
        Pass2,
    }

}