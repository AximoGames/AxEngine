// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aximo.Render.OpenGL
{
    public class ShaderCompilation : IObjectLabel
    {
        public List<ShaderSource> Sources = new List<ShaderSource>();
        public ShaderType Type;
        public int Handle { get; internal set; }
        public string ObjectLabel { get { return Path.GetFileName(Sources.First().Path); } set { } }

        public ObjectLabelIdentifier ObjectLabelIdentifier => ObjectLabelIdentifier.Shader;

        public string AllSources() => string.Join("\n", Sources.Select(s => s.Source));

        public void GenerateSource()
        {
            SetOrdinals();
            foreach (var source in Sources)
                source.GenerateSource();
        }

        public void SetOrdinals()
        {
            int ordinal = 0;
            foreach (var source in Sources)
                source.Ordinal = ordinal++;
        }

        public IDictionary<string, object> Defines { get; } = new Dictionary<string, object>();

        public void SetDefine(string name)
        {
            SetDefine(name, null);
        }

        public void SetDefine(string name, object value)
        {
            if (Defines.ContainsKey(name))
                Defines[name] = value;
            else
                Defines.Add(name, value);
        }
    }
}
