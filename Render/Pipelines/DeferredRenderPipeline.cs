// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Aximo.Render.OpenGL;
using Aximo.Render.VertexData;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render.Pipelines
{
    public class DeferredRenderPipeline : RenderPipeline
    {
        private FrameBuffer GBuffer;
        private RendererTexture GPosition;
        private RendererTexture GNormal;
        private RendererTexture GMaterial;
        private RendererTexture GAlbedoSpec;

        private RendererShader _DefLightShader;
        private VertexDataPos2UV[] _vertices = DataHelper.NDCQuadInvertedUV;

        private VertexArrayObject vao;
        private VertexBufferObject vbo;

        public override void Init()
        {
            var width = RenderContext.Current.ScreenSize.X;
            var height = RenderContext.Current.ScreenSize.Y;

            GBuffer = new FrameBuffer(width, height);
            GBuffer.ObjectLabel = nameof(GBuffer);
            //GBuffer.InitNormal();

            // R11fG11fB10f --> rgba16f
            // Warning: R11fG11fB10f has no sign bit!

            GPosition = new RendererTexture(nameof(GPosition), TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GPosition.SetNearestFilter();
            GBuffer.DestinationTextures.Add(GPosition);
            GBuffer.BindTexture(GPosition, FramebufferAttachment.ColorAttachment0);

            GNormal = new RendererTexture(nameof(GNormal), TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GNormal.SetNearestFilter();
            GBuffer.DestinationTextures.Add(GNormal);
            GBuffer.BindTexture(GNormal, FramebufferAttachment.ColorAttachment1);

            GAlbedoSpec = new RendererTexture(nameof(GAlbedoSpec), TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GAlbedoSpec.SetNearestFilter();
            GBuffer.DestinationTextures.Add(GAlbedoSpec);
            GBuffer.BindTexture(GAlbedoSpec, FramebufferAttachment.ColorAttachment2);

            GMaterial = new RendererTexture(nameof(GMaterial), TextureTarget.Texture2D, 0, PixelInternalFormat.R11fG11fB10f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GMaterial.SetNearestFilter();
            GBuffer.DestinationTextures.Add(GMaterial);
            GBuffer.BindTexture(GMaterial, FramebufferAttachment.ColorAttachment3);

            GL.DrawBuffers(4, new DrawBuffersEnum[]
            {
                DrawBuffersEnum.ColorAttachment0,
                DrawBuffersEnum.ColorAttachment1,
                DrawBuffersEnum.ColorAttachment2,
                DrawBuffersEnum.ColorAttachment3,
            });

            // var rboDepth = new RenderBuffer(gBuffer, RenderbufferStorage.DepthComponent, FramebufferAttachment.DepthAttachment);
            // rboDepth.ObjectLabel = nameof(rboDepth);

            // Attach default Forward Depth Buffer to this Framebuffer, so both share the same depth informations.
            var fwPipe = RenderContext.Current.GetPipeline<ForwardRenderPipeline>();
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, fwPipe.FrameBuffer.RenderBuffer.Handle);

            GBuffer.Check();

            _DefLightShader = new RendererShader("Shaders/deferred-shading.vert", "Shaders/deferred-shading.frag", null, false);
            if (Renderer.Current.UseShadows)
                _DefLightShader.SetDefine("USE_SHADOW");
            _DefLightShader.Compile();
            _DefLightShader.SetInt("gPosition", 0);
            _DefLightShader.SetInt("gNormal", 1);
            _DefLightShader.SetInt("gAlbedoSpec", 2);
            _DefLightShader.SetInt("gMaterial", 3);

            vbo = new VertexBufferObject();
            vbo.Create();
            vbo.Bind();

            var layout = VertexLayoutDefinition.CreateDefinitionFromVertexStruct<VertexDataPos2UV>();
            vao = new VertexArrayObject(layout.BindToShader(_DefLightShader), vbo);
            vao.Create();

            vao.SetData(BufferData.Create(_vertices));
        }

        public DeferredPass Pass;

        public override void Render(RenderContext context, Camera camera)
        {
            GL.Disable(EnableCap.Blend);

            ObjectManager.PushDebugGroup("OnRender Pass1", this);
            RenderPass1(context, camera);
            ObjectManager.PopDebugGroup();

            ObjectManager.PushDebugGroup("OnRender Pass2", this);
            RenderPass2(context, camera);
            ObjectManager.PopDebugGroup();

            GL.Enable(EnableCap.Blend);
        }

        private void RenderPass1(RenderContext context, Camera camera)
        {
            GL.Viewport(0, 0, context.ScreenSize.X, context.ScreenSize.Y);
            GBuffer.Bind();
            GL.Enable(EnableCap.DepthTest);

            // At least gPosition requires Color = 0, so the Positions is 0. Used to stencil the background.
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Pass = DeferredPass.Pass1;
            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

        public override IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera)
        {
            return SortFromFrontToBack(context, camera, base.GetRenderObjects(context, camera));
        }

        private void RenderPass2(RenderContext context, Camera camera)
        {
            Pass = DeferredPass.Pass2;
            ObjectManager.PushDebugGroup("OnRender LightShader", this);

            _DefLightShader.Bind();

            GPosition.Bind(TextureUnit.Texture0);
            GNormal.Bind(TextureUnit.Texture1);
            GAlbedoSpec.Bind(TextureUnit.Texture2);
            GMaterial.Bind(TextureUnit.Texture3);

            if (Renderer.Current.UseShadows)
            {
                context.GetPipeline<DirectionalShadowRenderPipeline>().FrameBuffer.GetDestinationTexture().Bind(TextureUnit.Texture4);
                _DefLightShader.SetInt("DirectionalShadowMap", 4);
                context.GetPipeline<PointShadowRenderPipeline>().FrameBuffer.GetDestinationTexture().Bind(TextureUnit.Texture5);
                _DefLightShader.SetInt("PointShadowMap", 5);
            }

            _DefLightShader.SetVector3("ViewPos", camera.Position);
            _DefLightShader.SetFloat("FarPlane", camera.FarPlane);

            // TODO: Move to Pass1
            //_DefLightShader.SetMaterial("material", Material.Default);

            _DefLightShader.BindBlock("LightsArray", context.LightBinding);
            _DefLightShader.SetInt("LightCount", context.LightObjects.Count);

            context.GetPipeline<ForwardRenderPipeline>().FrameBuffer.Bind();
            vao.Bind();
            GL.Disable(EnableCap.DepthTest);
            vao.Draw();
            GL.Enable(EnableCap.DepthTest);

            ObjectManager.PopDebugGroup();
        }

        public override void OnScreenResize(ScreenResizeEventArgs e)
        {
            GBuffer.Resize(e.Size.X, e.Size.Y);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //GBuffer.Dispose(
                GPosition.Dispose();
                GNormal.Dispose();
                GMaterial.Dispose();
                GAlbedoSpec.Dispose();

                //_DefLightShader.Dispose();
                //_vertices.dis;

                //vao;
                //vbo
            }

            GPosition = null;
            GNormal = null;
            GMaterial = null;
            GAlbedoSpec = null;

            base.Dispose(disposing);
        }
    }

    public enum DeferredPass
    {
        Pass1,
        Pass2,
    }
}
