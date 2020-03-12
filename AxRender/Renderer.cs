// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Aximo.Render;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace Aximo.Render
{
    public class Renderer
    {

        public static Renderer Current;
        public Vector2i ScreenSize;

        public RenderContext Context => RenderContext.Current;

        private static void PrintExtensions()
        {
            int numExtensions;
            GL.GetInteger(GetPName.NumExtensions, out numExtensions);
            for (var i = 0; i < numExtensions; i++)
            {
                var extName = GL.GetString(StringNameIndexed.Extensions, i);
                Console.WriteLine(extName);
            }
        }

        private DebugProc _debugProcCallback;
        private GCHandle _debugProcCallbackHandle;
        public static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            string messageString = Marshal.PtrToStringAnsi(message, length);

            Console.WriteLine($"{severity} {type} | {messageString}");

            if (type == DebugType.DebugTypeError)
            {
                throw new Exception(messageString);
            }
        }

        public void Init()
        {
            var vendor = GL.GetString(StringName.Vendor);
            var version = GL.GetString(StringName.Version);
            var shadingLanguageVersion = GL.GetString(StringName.ShadingLanguageVersion);
            var renderer = GL.GetString(StringName.Renderer);

            Console.WriteLine($"Vendor: {vendor}, version: {version}, shadinglangVersion: {shadingLanguageVersion}, renderer: {renderer}");

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            ObjectManager.PushDebugGroup("Setup", "Pipelines");
            SetupPipelines();
            ObjectManager.PopDebugGroup();

            PrimaryRenderPipeline = GetPipeline<DeferredRenderPipeline>();
        }

        public void AddPipeline(IRenderPipeline pipeline)
        {
            RenderPipelines.Add(pipeline);
        }

        public List<IRenderPipeline> RenderPipelines = new List<IRenderPipeline>();

        public IRenderPipeline CurrentPipeline { get; internal set; }

        public void SetupPipelines()
        {
            AddPipeline(new DirectionalShadowRenderPipeline());
            AddPipeline(new PointShadowRenderPipeline());
            AddPipeline(new DeferredRenderPipeline());
            AddPipeline(new ForwardRenderPipeline());
            AddPipeline(new ScreenPipeline());

            ObjectManager.PushDebugGroup("BeforeInit", "Pipelines");
            foreach (var pipe in RenderPipelines)
            {
                ObjectManager.PushDebugGroup("BeforeInit", pipe);
                pipe.BeforeInit();
                ObjectManager.PopDebugGroup();
            }
            ObjectManager.PopDebugGroup();

            ObjectManager.PushDebugGroup("Init", "Pipelines");
            foreach (var pipe in RenderPipelines)
            {
                ObjectManager.PushDebugGroup("Init", pipe);
                pipe.Init();
                ObjectManager.PopDebugGroup();
            }
            ObjectManager.PopDebugGroup();

            ObjectManager.PushDebugGroup("AfterInit", "Pipelines");
            foreach (var pipe in RenderPipelines)
            {
                ObjectManager.PushDebugGroup("AfterInit", pipe);
                pipe.AfterInit();
                ObjectManager.PopDebugGroup();
            }
            ObjectManager.PopDebugGroup();
        }

        public T GetPipeline<T>()
            where T : class, IRenderPipeline
        {
            return (T)RenderPipelines.FirstOrDefault(p => p is T);
        }

        public IRenderPipeline PrimaryRenderPipeline { get; set; }

        public void OnScreenResize(Vector2i size)
        {
            ScreenSize = size;
            GL.Viewport(0, 0, ScreenSize.X, ScreenSize.Y);

            // GL.Scissor(0, 0, ScreenSize.X, ScreenSize.Y);
            // GL.Enable(EnableCap.ScissorTest);

            foreach (var pipe in Renderer.Current.RenderPipelines)
                pipe.OnScreenResize();

            Context.OnScreenResize();
        }

        public void InitRender()
        {
            foreach (var pipeline in RenderPipelines)
            {
                ObjectManager.PushDebugGroup("InitRender", pipeline);
                CurrentPipeline = pipeline;
                pipeline.InitRender(Context, Context.Camera);
                ObjectManager.PopDebugGroup();
            }
        }

        public void Render()
        {
            foreach (var pipeline in RenderPipelines)
            {
                ObjectManager.PushDebugGroup("Render", pipeline);
                CurrentPipeline = pipeline;
                pipeline.Render(Context, Context.Camera);
                ObjectManager.PopDebugGroup();
            }
        }

    }

}
