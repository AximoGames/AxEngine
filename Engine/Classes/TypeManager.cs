using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using McMaster.NETCore.Plugins;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    internal static class TypeManager
    {
        public static Type GetType(string name)
        {
            var componentType = PluginManager.GetScriptBehaviourType(name);

            if (componentType != null)
                return componentType;

            throw new NotImplementedException();
        }

        public static bool OwnedByDefaultLoadContext(Type type)
        {
            return false;
        }

    }

}
