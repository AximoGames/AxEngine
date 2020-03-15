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

            //PrintExtensions();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        public void SetupPipelines()
        {
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
            if (RenderContext.LightObjects.Count >= 2)
            {
                var lightsData = new GlslLight[2];
                lightsData[0].Position = RenderContext.LightObjects[0].Position;
                lightsData[0].Color = new Vector3(0.5f, 0.5f, 0.5f);
                lightsData[0].ShadowLayer = RenderContext.LightObjects[0].ShadowTextureIndex;
                lightsData[0].DirectionalLight = RenderContext.LightObjects[0].LightType == LightType.Directional ? 1 : 0;
                lightsData[0].LightSpaceMatrix = Matrix4.Transpose(RenderContext.LightObjects[0].LightCamera.ViewMatrix * RenderContext.LightObjects[0].LightCamera.ProjectionMatrix);
                lightsData[0].Linear = 0.1f;
                lightsData[0].Quadric = 0f;

                lightsData[1].Position = RenderContext.LightObjects[1].Position;
                lightsData[1].Color = new Vector3(0.5f, 0.5f, 0.5f);
                lightsData[1].ShadowLayer = RenderContext.LightObjects[1].ShadowTextureIndex;
                lightsData[1].DirectionalLight = RenderContext.LightObjects[1].LightType == LightType.Directional ? 1 : 0;
                lightsData[1].LightSpaceMatrix = Matrix4.Transpose(RenderContext.LightObjects[1].LightCamera.ViewMatrix * RenderContext.LightObjects[1].LightCamera.ProjectionMatrix);
                lightsData[1].Linear = 0.1f;
                lightsData[1].Quadric = 0f;
                ubo.SetData(lightsData);

                ubo.SetBindingPoint(RenderContext.LightBinding);
            }

            //--
            GL.Enable(EnableCap.DepthTest);

            //--

            RenderContext.InitRender();
            RenderContext.Render();

            //--

            // Configure

            // Render objects

            // Render Screen Surface

            //CheckForProgramError();

            ubo.Free();
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
        [StructLayout(LayoutKind.Explicit, Size = 112)]
        private struct GlslLight
        {
            [FieldOffset(0)]
            public Vector3 Position;

            [FieldOffset(16)]
            public Vector3 Color;

            [FieldOffset(32)]
            public Matrix4 LightSpaceMatrix;

            [FieldOffset(96)]
            public int ShadowLayer;

            [FieldOffset(100)]
            public int DirectionalLight; // Bool

            [FieldOffset(104)]
            public float Linear;

            [FieldOffset(108)]
            public float Quadric;
        }

    }

}
