// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Aximo.Render.Objects;
using Aximo.Render.OpenGL;
using Aximo.Render.Pipelines;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public class Renderer
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<Renderer>();

        public static Renderer Current;
        public Vector2i ScreenSize;
        public FlushRenderBackend FlushRenderBackend;
        public bool UseShadows;
        public bool UseFrameDebug;

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

        public void Init()
        {
            //PrintExtensions();
            CheckMemoryLeak();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            InternalTextureManager.Init();

            ObjectManager.PushDebugGroup("Setup", "Pipelines");
            SetupPipelines();
            ObjectManager.PopDebugGroup();
            RenderContext.PrimaryRenderPipeline = RenderContext.GetPipeline<DeferredRenderPipeline>();

            RenderContext.LightBinding = new BindingPoint();
            Log.Verbose("LightBinding: {Number}", RenderContext.LightBinding.Number);

            // ctx.Camera = new OrthographicCamera(new Vector3(1f, -5f, 2f))
            // {
            //     NearPlane = 0.01f,
            //     FarPlane = 100.0f,
            // };

            RenderContext.AddObject(new ScreenSceneObject()
            {
            });
        }

        public void SetupPipelines()
        {
            if (UseShadows)
            {
                RenderContext.AddPipeline(new DirectionalShadowRenderPipeline());
                RenderContext.AddPipeline(new PointShadowRenderPipeline());
            }
            RenderContext.AddPipeline(new DeferredRenderPipeline());
            RenderContext.AddPipeline(new ForwardRenderPipeline());
            RenderContext.AddPipeline(new ScreenPipeline());

            ObjectManager.PushDebugGroup("BeforeInit", "Pipelines");
            foreach (var pipe in RenderContext.RenderPipelines)
            {
                ObjectManager.PushDebugGroup("BeforeInit", pipe);
                pipe.BeforeInit();
                ObjectManager.PopDebugGroup();
            }
            ObjectManager.PopDebugGroup();

            ObjectManager.PushDebugGroup("Init", "Pipelines");
            foreach (var pipe in RenderContext.RenderPipelines)
            {
                ObjectManager.PushDebugGroup("Init", pipe);
                pipe.Init();
                ObjectManager.PopDebugGroup();
            }
            ObjectManager.PopDebugGroup();

            ObjectManager.PushDebugGroup("AfterInit", "Pipelines");
            foreach (var pipe in RenderContext.RenderPipelines)
            {
                ObjectManager.PushDebugGroup("AfterInit", pipe);
                pipe.AfterInit();
                ObjectManager.PopDebugGroup();
            }
            ObjectManager.PopDebugGroup();
        }

        private void CheckMemoryLeak()
        {
            if (InternalTextureManager.ReferencedCount() > 0)
                Log.Warning("{Path}: {Value}", $"{nameof(InternalTextureManager)}.{nameof(InternalTextureManager.ReferencedCount)}", InternalTextureManager.ReferencedCount());
        }

        public void OnScreenResize(Vector2i size)
        {
        }

        public RenderContext RenderContext => RenderContext.Current;

        public void Render()
        {
            //--
            var ubo = new UniformBufferObject();
            ubo.Create();
            // if (RenderContext.LightObjects.Count >= 2)
            // {
            //     var lightsData = new GlslLight[2];
            //     lightsData[0].Position = RenderContext.LightObjects[0].Position;
            //     lightsData[0].Color = new Vector3(0.5f, 0.5f, 0.5f);
            //     lightsData[0].ShadowLayer = RenderContext.LightObjects[0].ShadowTextureIndex;
            //     lightsData[0].DirectionalLight = RenderContext.LightObjects[0].LightType == LightType.Directional ? 1 : 0;
            //     lightsData[0].LightSpaceMatrix = Matrix4.Transpose(RenderContext.LightObjects[0].LightCamera.ViewMatrix * RenderContext.LightObjects[0].LightCamera.ProjectionMatrix);
            //     lightsData[0].Linear = 0.1f;
            //     lightsData[0].Quadric = 0f;

            //     lightsData[1].Position = RenderContext.LightObjects[1].Position;
            //     lightsData[1].Color = new Vector3(0.5f, 0.5f, 0.5f);
            //     lightsData[1].ShadowLayer = RenderContext.LightObjects[1].ShadowTextureIndex;
            //     lightsData[1].DirectionalLight = RenderContext.LightObjects[1].LightType == LightType.Directional ? 1 : 0;
            //     lightsData[1].LightSpaceMatrix = Matrix4.Transpose(RenderContext.LightObjects[1].LightCamera.ViewMatrix * RenderContext.LightObjects[1].LightCamera.ProjectionMatrix);
            //     lightsData[1].Linear = 0.1f;
            //     lightsData[1].Quadric = 0f;
            //     ubo.SetData(BufferData.Create(lightsData));

            //     ubo.SetBindingPoint(RenderContext.LightBinding);
            // }

            if (RenderContext.LightObjects.Count > 0)
            {
                var lightDataList = new List<GlslLight>();
                foreach (var light in RenderContext.LightObjects)
                {
                    var lightData = new GlslLight();
                    lightData.Position = light.Position;
                    lightData.Color = light.Color;
                    lightData.ShadowLayer = light.ShadowTextureIndex;
                    lightData.DirectionalLight = light.LightType == LightType.Directional ? 1 : 0;
                    lightData.LightSpaceMatrix = Matrix4.Transpose(light.LightCamera.ViewMatrix * light.LightCamera.ProjectionMatrix);
                    lightData.Linear = light.Linear;
                    lightData.Quadric = light.Quadric;
                    lightData.FarPlane = light.LightCamera.FarPlane;
                    lightData.Direction = light.Direction.Normalized();
                    lightDataList.Add(lightData);
                }
                ubo.SetData(BufferData.Create(lightDataList.ToArray()));
                ubo.SetBindingPoint(RenderContext.LightBinding);
            }

            //--
            GL.Enable(EnableCap.DepthTest);

            //--

            RenderContext.InitRender();
            RenderContext.Render();
            RenderContext.OnWorldRendered();

            //--

            // Configure

            // Render objects

            // Render Screen Surface

            //CheckForProgramError();

            ubo.Free();

            if (FlushRenderBackend == FlushRenderBackend.End)
                GL.Finish();
        }

        private void CheckForProgramError()
        {
            var err = LastErrorCode;
            if (err != ErrorCode.NoError)
            {
                var s = "".ToString();
            }
        }

        public static ErrorCode LastErrorCode => GL.GetError();

        [Serializable]
        [StructLayout(LayoutKind.Explicit, Size = 128)]
        private struct GlslLight
        {
            [FieldOffset(0)]
            public Vector3 Position;

            [FieldOffset(16)]
            public Vector4 Color;

            [FieldOffset(32)]
            public Matrix4 LightSpaceMatrix;

            [FieldOffset(96)]
            public Vector3 Direction;

            [FieldOffset(108)]
            public int ShadowLayer;

            [FieldOffset(112)]
            public int DirectionalLight; // Bool

            [FieldOffset(116)]
            public float Linear;

            [FieldOffset(120)]
            public float Quadric;

            [FieldOffset(124)]
            public float FarPlane;
        }
    }
}
