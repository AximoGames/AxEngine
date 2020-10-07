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

    public static class TypeExtensions
    {
        public static bool Is<T>(this Type self)
        {
            return typeof(T).IsAssignableFrom(self);
        }

        public static bool Is(this Type self, Type otherType)
        {
            return otherType.IsAssignableFrom(self);
        }
    }

}
