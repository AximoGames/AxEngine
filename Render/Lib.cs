// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

using OpenToolkit;
using OpenToolkit.Mathematics;

#pragma warning disable SA1649 // File name should match first type name

namespace Aximo.Render
{

    public interface IRenderObject : IData
    {
        int Id { get; }
        string Name { get; set; }
        bool Enabled { get; set; }
        bool Orphaned { get; set; }
        int Order { get; set; }
        RenderContext Context { get; }
        void Init();
        void Free();
        void AssignContext(RenderContext ctx);
        void OnScreenResize(ScreenResizeEventArgs e);
        void OnWorldRendered();
    }

    public interface IData
    {
        T GetExtraData<T>(string name, T defaultValue = default);
        bool HasExtraData(string name);
        bool SetExraData<T>(string name, T value, T defaultValue = default);
    }

    internal static class IDataHelper
    {
        public static T GetData<T>(Dictionary<string, object> data, string name, T defaultValue = default)
        {
            if (data.TryGetValue(name, out object value))
                return (T)value;
            return default;
        }

        public static bool HasData(Dictionary<string, object> data, string name)
        {
            return data.ContainsKey(name);
        }

        public static bool SetData<T>(Dictionary<string, object> data, string name, T value, T defaultValue = default)
        {
            if (data.TryGetValue(name, out object currentValue))
            {
                if (object.Equals(value, defaultValue))
                {
                    data.Remove(name);
                    return true;
                }
                else
                {
                    if (object.Equals(currentValue, value))
                        return false;

                    data[name] = value;
                    return true;
                }
            }
            else
            {
                if (object.Equals(value, defaultValue))
                    return false;

                data.Add(name, value);
                return true;
            }
        }

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

    public interface IReloadable : IRenderObject
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

    public interface IPosition
    {
        Vector3 Position { get; set; }
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
        Vector3 Color { get; set; }
        float Linear { get; set; }
        float Quadric { get; set; }
    }

    public enum LightType
    {
        Directional,
        Point,
    }

}
