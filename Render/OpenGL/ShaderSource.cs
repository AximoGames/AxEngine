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
    public class ShaderSource
    {
        public string Path;
        public string OriginalSource;
        public string Source;
        public int Ordinal;
        public ShaderCompilation Compilation;

        private static Regex FileFinder = new Regex(@"#include ""([\w\.\/-]+)""", RegexOptions.RightToLeft);
        private static Regex DefineFileFinder = new Regex(@"#include ([\w_]+)", RegexOptions.RightToLeft);

        public void GenerateSource()
        {
            Source = LoadSource(Path, ref OriginalSource, this);
        }

        // Just loads the entire file into a string.
        private static string LoadSource(string path, ref string content, ShaderSource sh, bool isIncludeFile = false)
        {
            var loadedContent = LoadFile(path, content);
            content = loadedContent;

            if (sh.Ordinal == 0)
            {
                var lines = loadedContent.Split(Environment.NewLine).ToList();
                foreach (var entry in sh.Compilation.Defines)
                {
                    var defineLine = "#define " + entry.Key;
                    var value = entry.Value;
                    if (value == null)
                        value = 1;
                    defineLine += " " + GetDefineLiteral(value);
                    lines.Insert(1, defineLine);
                }
                loadedContent = string.Join(Environment.NewLine, lines);
            }

            var sb = new StringBuilder(loadedContent);

            // replaces #include DEFINED_MACRO
            foreach (Match match in DefineFileFinder.Matches(sb.ToString()))
            {
                var filePlaceholder = match.Groups[1].Value;
                if (sh.Compilation.Defines.ContainsKey(filePlaceholder))
                    sb.Replace(match.Value, $"#include {GetDefineLiteral(sh.Compilation.Defines[filePlaceholder])}", match.Index, match.Length);
            }

            // replaces #include "Path to file"
            foreach (Match match in FileFinder.Matches(sb.ToString()))
            {
                var includePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(path), match.Groups[1].Value);
                var dummy = "";
                sb.Replace(match.Value, LoadSource(includePath, ref dummy, sh, true), match.Index, match.Length);
            }
            return AddFileNameGroup(sb.ToString(), path);
        }

        private static string AddFileNameGroup(string source, string label)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("// --- BEGIN " + label + " ---");
            sb.AppendLine();
            sb.Append(source);
            sb.AppendLine();
            sb.AppendLine("// --- END " + label + " ---");
            sb.AppendLine();
            return sb.ToString();
        }

        private static string GetDefineLiteral(object value)
        {
            if (value == null)
                return "";

            if (value is string)
            {
                return "\"" + value.ToString() + "\"";
            }
            else
            {
                return value.ToString();
            }
        }

        // Just loads the entire file into a string.
        private static string LoadFile(string path = null, string content = null)
        {
            if (!string.IsNullOrEmpty(content))
                return content;

            var absPath = AssetManager.GetAssetsPath(path);
            if (string.IsNullOrEmpty(absPath))
                throw new Exception("Could not load file: " + path);

            using (var sr = new StreamReader(absPath, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
