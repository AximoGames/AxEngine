using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using McMaster.NETCore.Plugins;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

#nullable disable

namespace Aximo.Engine
{

    public static class PluginManager
    {

        private static PluginLoader Loader;
        private static string File = @"C:\Users\sebastian\projects\AximoGames\AxEngine\Demo.Plugin\bin\Debug\netcoreapp3.1\AxDemo.Plugin.dll";

        static PluginManager()
        {
            Load();
        }

        private static void Load()
        {
            Loader = PluginLoader.CreateFromAssemblyFile(File,
                config =>
                {
                    config.IsUnloadable = true;
                    config.PreferSharedTypes = true;
                    config.LoadInMemory = true;
                });
        }

        public static Type GetScriptBehaviourType(string name)
        {
            return Loader.LoadDefaultAssembly().GetType($"Aximo.AxDemo.{name}");
        }

        public static IScriptBehaviour GetScriptBehaviour(string name)
        {
            var t = GetScriptBehaviourType(name);
            var obj = Activator.CreateInstance(t);
            return (IScriptBehaviour)obj;
        }

        public static IScriptBehaviour GetScriptBehaviour<T>()
        {
            return GetScriptBehaviour(typeof(T));
        }

        public static IScriptBehaviour GetScriptBehaviour(Type type)
        {
            // It's important to use the name, because T may be from the host assembly.
            return GetScriptBehaviour(type.Name);
        }

        public static void Reload()
        {
            Loader.Reload();

            var actors = SceneManager.GetCurrentScene().GetActors();
            var scriptsToReload = new List<ScriptBehaviourWrapper>();
            foreach (var act in actors)
                foreach (ScriptBehaviourWrapper script in act.GetComponents().Where(c => c is ScriptBehaviourWrapper))
                    scriptsToReload.Add(script);

            foreach (var script in scriptsToReload)
                script.ReloadStage1();

            foreach (var script in scriptsToReload)
            {
                var newScript = GetScriptBehaviour(script.Script.GetType());
                script.ReloadStage2(newScript);
            }

            foreach (var script in scriptsToReload)
                script.ReloadStage3();
        }

    }

}
