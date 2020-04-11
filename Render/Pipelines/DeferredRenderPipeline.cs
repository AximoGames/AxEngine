﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
{
    public class DeferredRenderPipeline : RenderPipeline
    {

        public FrameBuffer GBuffer;
        public Texture GPosition;
        public Texture GNormal;
        public Texture GMaterial;
        public Texture GAlbedoSpec;

        private Shader _DefLightShader;
        private float[] _vertices = DataHelper.Quad;

        private VertexArrayObject vao;
        private VertexBufferObject vbo;

        public override void Init()
        {
            var width = RenderContext.Current.ScreenSize.X;
            var height = RenderContext.Current.ScreenSize.Y;

            GBuffer = new FrameBuffer(width, height);
            GBuffer.ObjectLabel = nameof(GBuffer);
            GBuffer.InitNormal();

            GPosition = new Texture(nameof(GPosition), TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GPosition.SetNearestFilter();
            GBuffer.DestinationTextures.Add(GPosition);
            GBuffer.BindTexture(GPosition, FramebufferAttachment.ColorAttachment0);

            GNormal = new Texture(nameof(GNormal), TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
            GNormal.SetNearestFilter();
            GBuffer.DestinationTextures.Add(GNormal);
            GBuffer.BindTexture(GNormal, FramebufferAttachment.ColorAttachment1);

            GAlbedoSpec = new Texture(nameof(GAlbedoSpec), TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GAlbedoSpec.SetNearestFilter();
            GBuffer.DestinationTextures.Add(GAlbedoSpec);
            GBuffer.BindTexture(GAlbedoSpec, FramebufferAttachment.ColorAttachment2);

            GMaterial = new Texture(nameof(GMaterial), TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, width, height, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
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

            _DefLightShader = new Shader("Shaders/deferred-shading.vert", "Shaders/deferred-shading.frag");
            _DefLightShader.SetInt("gPosition", 0);
            _DefLightShader.SetInt("gNormal", 1);
            _DefLightShader.SetInt("gAlbedoSpec", 2);
            _DefLightShader.SetInt("gMaterial", 3);

            vbo = new VertexBufferObject();
            vbo.Create();
            vbo.Bind();

            var layout = new VertexLayoutBinded();
            layout.AddAttribute<float>(_DefLightShader.GetAttribLocation("aPos"), 2);
            layout.AddAttribute<float>(_DefLightShader.GetAttribLocation("aTexCoords"), 2);

            vao = new VertexArrayObject(layout, vbo);
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
            //GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // At least gPosition requires Color = 0, so the Positions is 0. Used to stencil the background.
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Pass = DeferredPass.Pass1;
            foreach (var obj in GetRenderObjects(context, camera))
                Render(context, camera, obj);
        }

        private void RenderPass2(RenderContext context, Camera camera)
        {
            Pass = DeferredPass.Pass2;
            // GL.ClearColor(0.1f, 0.3f, 0.3f, 1.0f);
            // GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Needed?
            // context.GetPipeline<ForwardRenderPipeline>().FrameBuffer.Use();
            // foreach (var obj in GetRenderObjects(context, camera))
            //     Render(context, camera, obj);

            ObjectManager.PushDebugGroup("OnRender LightShader", this);

            _DefLightShader.Bind();

            GPosition.Bind(TextureUnit.Texture0);
            GNormal.Bind(TextureUnit.Texture1);
            GAlbedoSpec.Bind(TextureUnit.Texture2);
            GMaterial.Bind(TextureUnit.Texture3);

            context.GetPipeline<DirectionalShadowRenderPipeline>().FrameBuffer.GetDestinationTexture().Bind(TextureUnit.Texture4);
            context.GetPipeline<PointShadowRenderPipeline>().FrameBuffer.GetDestinationTexture().Bind(TextureUnit.Texture5);

            _DefLightShader.SetVector3("ViewPos", camera.Position);
            _DefLightShader.SetFloat("FarPlane", camera.FarPlane);

            // TODO: Move to Pass1
            //_DefLightShader.SetMaterial("material", Material.Default);

            _DefLightShader.SetInt("DirectionalShadowMap", 4);
            _DefLightShader.SetInt("PointShadowMap", 5);
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
    }

    public enum DeferredPass
    {
        Pass1,
        Pass2,
    }

}