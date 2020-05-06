// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

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

        public override IEnumerable<IRenderableObject> GetRenderObjects(RenderContext context, Camera camera)
        {
            var objects = base.GetRenderObjects(context, camera).ToList();
            objects.Sort(new MeshSorter(camera));
            return objects;
        }

        private class MeshSorter : IComparer<IRenderableObject>
        {
            public Camera Camera;
            private Vector3 CameraPosition;

            public MeshSorter(Camera camera)
            {
                Camera = camera;
                CameraPosition = camera.Position;
            }

            // returns: Draw-Order. 
            public int Compare(IRenderableObject x, IRenderableObject y)
            {
                // Draw Early: -1.
                // Draw Late: 1
                var boundsX = x as IBounds;
                var boundsY = y as IBounds;
                var hasBoundsX = boundsX != null ? 1 : 0;
                var hasBoundsY = boundsY != null ? 1 : 0;
                if (hasBoundsX != hasBoundsY)
                    return hasBoundsX.CompareTo(hasBoundsY);

                if (hasBoundsX == 0) // no check for Y needed
                    return 0;

                var distanceX = Vector3.Distance(CameraPosition, boundsX.WorldBounds.Center);
                var distanceY = Vector3.Distance(CameraPosition, boundsY.WorldBounds.Center);
                return -distanceX.CompareTo(distanceY);
            }
        }

        public override void OnScreenResize(ScreenResizeEventArgs e)
        {
            // if (RenderContext.Current.ScreenSize.X == 100)
            //     throw new Exception();
            FrameBuffer.Resize(e.Size.X, e.Size.Y);
        }
    }
}