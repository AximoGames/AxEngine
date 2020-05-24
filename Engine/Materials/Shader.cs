// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Aximo.Render;
using OpenToolkit;

namespace Aximo.Engine
{
    public class Shader
    {
        public string VertexShaderPath;
        public string FragmentShaderPath;
        public string GeometryShaderPath;

        public Shader(string vertexShaderPath, string fragmentShaderPath, string geometryShaderPath = null)
        {
            VertexShaderPath = vertexShaderPath;
            FragmentShaderPath = fragmentShaderPath;
            GeometryShaderPath = geometryShaderPath;
        }
    }
}
