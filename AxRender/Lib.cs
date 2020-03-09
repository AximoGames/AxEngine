// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

using OpenTK;

namespace Aximo.Render
{

    public interface IGameObject : IData
    {
        int Id { get; }
        string Name { get; set; }
        bool Enabled { get; set; }
        RenderContext Context { get; }
        void Init();
        void Free();
        void AssignContext(RenderContext ctx);
        void OnScreenResize();
    }

    public interface IData
    {
        T GetData<T>(string name, T defaultValue = default);
        bool HasData(string name);
        bool SetData<T>(string name, T value, T defaultValue = default);
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

    public interface IRenderableObject : IGameObject
    {
        void OnRender();
        List<IRenderPipeline> RenderPipelines { get; set; }
    }

    public interface IForwardRenderable : IRenderableObject
    {
        void OnForwardRender();
    }

    public interface IDeferredRenderable : IRenderableObject
    {
        void OnDeferredRender();
    }

    public interface ILightTarget
    {
        List<ILightObject> Lights { get; }
    }

    public interface IReloadable : IGameObject
    {
        void OnReload();
    }

    public interface IShadowObject : IGameObject, IRenderableObject
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
        Vector3 Rotate { get; set; }
    }

    public interface ILightObject : IPosition, IGameObject
    {
        bool Shadows { get; set; }
        int ShadowTextureIndex { get; set; }
        Camera LightCamera { get; }
        LightType LightType { get; set; }
    }

    public enum LightType
    {
        Directional,
        Point,
    }

}
