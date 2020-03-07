using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using System.IO;

namespace AxEngine
{


    public static class DirectoryHelper
    {
        private static string _AppRootDir;
        public static string AppRootDir
        {
            get
            {
                if (_AppRootDir == null)
                    _AppRootDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..")).FullName;
                return _AppRootDir;
            }
        }

        private static string _EngineRootDir;
        public static string EngineRootDir
        {
            get
            {
                if (_EngineRootDir == null)
                {
                    var dirName = Path.GetFileName(AppRootDir);
                    if (dirName == "AxEngine")
                    {
                        _EngineRootDir = AppRootDir;
                    }
                    else
                    {
                        _EngineRootDir = new DirectoryInfo(Path.Combine(AppRootDir, "..", "..", "..", "AxEngine", "src", "AxEngine")).FullName;
                    }
                }
                return _EngineRootDir;
            }
        }

        private static List<string> _SearchDirectories;
        public static List<string> SearchDirectories
        {
            get
            {
                if (_SearchDirectories == null)
                    _SearchDirectories = new List<string> { AppRootDir, EngineRootDir };
                return _SearchDirectories;
            }
        }

        public static string GetPath(string subPath)
        {
            foreach (var dir in SearchDirectories)
            {
                var path = Path.Combine(dir, subPath);
                if (File.Exists(path))
                    return new FileInfo(path).FullName;
                if (Directory.Exists(path))
                    return new DirectoryInfo(path).FullName;
            }
            return "";
        }

    }

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
