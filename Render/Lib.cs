// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Aximo.Render.Pipelines;
using OpenToolkit.Mathematics;

#pragma warning disable SA1649 // File name should match first type name

namespace Aximo.Render
{
    public interface IRenderObject : IData, IDisposable
    {
        int Id { get; }
        string Name { get; set; }
        bool Enabled { get; set; }
        bool Orphaned { get; set; }
        int DrawPriority { get; set; }
        bool UseTransparency { get; set; }
        RenderContext Context { get; }
        void Init();
        void Free();
        void AssignContext(RenderContext ctx);
        void OnScreenResize(ScreenResizeEventArgs e);
        void OnWorldRendered();
    }

    public interface IRenderableObject : IRenderObject
    {
        void OnRender();
        List<IRenderPipeline> RenderPipelines { get; }
    }

    public interface IForwardRenderable : IRenderableObject
    {
        void OnForwardRender();
    }

    public interface IScreenRenderable : IRenderableObject
    {
        void OnScreenRender();
    }

    public interface IDeferredRenderable : IRenderableObject
    {
        void OnDeferredRender();
    }

    public interface ILightTarget
    {
        List<ILightObject> Lights { get; }
    }

    public interface IReloadable
    {
        void OnReload();
    }

    public interface IShadowObject : IRenderObject, IRenderableObject
    {
        void OnRenderShadow();
        void OnRenderCubeShadow();
        bool RenderShadow { get; set; }
    }

    public interface IRenderTarget : IRenderableObject
    {
    }

    public interface IUpdateFrame
    {
        void OnUpdateFrame();
    }

    public interface IMeshObject : IRenderableObject
    {
        Vector3[] GetVertices();
        int[] GetIndices();
    }

    public interface IBounds
    {
        Box3 WorldBounds { get; set; }
    }

    public interface IScaleRotate : IPosition
    {
        Vector3 Scale { get; set; }
        Quaternion Rotate { get; set; }
    }

    public interface ILightObject : IPosition, IRenderObject
    {
        bool Shadows { get; set; }
        int ShadowTextureIndex { get; set; }
        Camera LightCamera { get; }
        LightType LightType { get; set; }
        Vector4 Color { get; set; }
        float Linear { get; set; }
        float Quadric { get; set; }
        Vector3 Direction { get; set; }
    }

    public enum LightType
    {
        Directional,
        Point,
    }
}
