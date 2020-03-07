using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using AxEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace AxEngine
{
    // A simple class meant to help create shaders.
    public class Shader : IObjectLabel
    {
        public int Handle { get; private set; }
        public string ObjectLabel { get { return Compilations.FirstOrDefault()?.ObjectLabel ?? ""; } set { } }

        public ObjectLabelIdentifier ObjectLabelIdentifier => ObjectLabelIdentifier.Program;

        private Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();
        private Dictionary<string, int> _uniformBlockLocations = new Dictionary<string, int>();

        private List<ShaderCompilation> Compilations = new List<ShaderCompilation>();

        public void SetDefine(string name)
        {
            SetDefine(name, null);
        }

        public void SetDefine(string name, object value)
        {
            foreach (var comp in Compilations)
                comp.SetDefine(name, value);
        }

        public void AddSource(string path, ShaderType type)
        {
            ShaderCompilation comp = Compilations.FirstOrDefault(c => c.Type == type);
            if (comp == null)
                Compilations.Add(comp = new ShaderCompilation { Type = type });

            comp.Sources.Add(new ShaderSource
            {
                Path = path,
                Compilation = comp,
            });
            comp.SetOrdinals();
        }

        public Shader()
        {
        }

        // This is how you create a simple shader.
        // Shaders are written in GLSL, which is a language very similar to C in its semantics.
        // The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
        // A commented example of GLSL can be found in shader.vert
        public Shader(string vertPath, string fragPath, string geomPath = null, bool compile = true)
        {
            AddSource(vertPath, ShaderType.VertexShader);
            AddSource(fragPath, ShaderType.FragmentShader);
            if (geomPath != null)
                AddSource(geomPath, ShaderType.GeometryShader);

            // foreach (var comp in Sources)
            // {
            //     comp.Sources.Insert(0, new ShaderSource
            //     {
            //         Path = "Shaders/lib.frag",
            //         Source = LoadSource("Shaders/lib.frag"),
            //     });
            // }

            if (compile)
                Compile();
        }

        public void Compile()
        {
            // There are several different types of shaders, but the only two you need for basic rendering are the vertex and fragment shaders.
            // The vertex shader is responsible for moving around vertices, and uploading that data to the fragment shader.
            //   The vertex shader won't be too important here, but they'll be more important later.
            // The fragment shader is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.
            //   The fragment shader is what we'll be using the most here.

            // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
            foreach (var comp in Compilations)
            {
                comp.GenerateSource();

                comp.Handle = GL.CreateShader(comp.Type);
                var shaderSources = comp.Sources.Select(s => s.Source).ToArray();

                var len = shaderSources.Select(s => s.Length).ToArray();

                // Now, bind the GLSL source code
                GL.ShaderSource(comp.Handle, shaderSources.Length, shaderSources, len);

                // And then compile
                CompileShader(comp);
            }

            // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
            // To do this, create a program...
            Handle = GL.CreateProgram();

            // Attach both shaders...

            foreach (var comp in Compilations)
                GL.AttachShader(Handle, comp.Handle);

            // And then link them together.
            LinkProgram(Handle);

            Bind();
            ObjectManager.SetLabel(this);

            //if(

            // When the shader program is linked, it no longer needs the individual shaders attacked to it; the compiled code is copied into the shader program.
            // Detach them, and then delete them.

            foreach (var comp in Compilations)
            {
                GL.DetachShader(Handle, comp.Handle);
                GL.DeleteShader(comp.Handle);
            }
            // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
            // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
            // later.

            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniforms; i++)
            {
                // get the name of this uniform,
                int size;
                ActiveUniformType type;
                var key = GL.GetActiveUniform(Handle, i, out size, out type);

                // get the location,
                var location = GL.GetUniformLocation(Handle, key);

                // and then add it to the dictionary.
                _uniformLocations.Add(key, location);

                if (size > 1)
                {
                    for (int n = 1; n < size; n++)
                    {
                        var keyN = key.Replace("[0]", $"[{n}]");
                        _uniformLocations.Add(keyN, GL.GetUniformLocation(Handle, keyN));
                    }
                }

            }

            // First, we have to get the number of active uniforms in the shader.
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniformBlocks, out var numberOfUniformBlocks);
            // Loop over all the uniforms,
            for (var i = 0; i < numberOfUniformBlocks; i++)
            {
                // get the name of this uniform,
                GL.GetActiveUniformBlock(Handle, i, ActiveUniformBlockParameter.UniformBlockNameLength, out var blockNameLen);
                GL.GetActiveUniformBlockName(Handle, i, blockNameLen, out var len, out var key);

                // get the location,
                var location = GL.GetUniformBlockIndex(Handle, key);

                // and then add it to the dictionary.
                _uniformBlockLocations.Add(key, location);
            }

        }

        public void Reload()
        {
            try
            {
                var sh = new Shader();
                foreach (var comp in Compilations)
                    foreach (var src in comp.Sources)
                        sh.AddSource(src.Path, comp.Type);

                sh.Compile();
                SetFrom(sh);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void SetFrom(Shader source)
        {
            Handle = source.Handle;
            Compilations = source.Compilations;
            _uniformLocations = source._uniformLocations;
        }

        private static void CompileShader(ShaderCompilation comp)
        {
            var shader = comp.Handle;

            // Try to compile the shader
            GL.CompileShader(shader);

            // Check for compilation errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var msg = GL.GetShaderInfoLog(shader);
                Console.WriteLine(msg);
                if (Directory.Exists("/tmp"))
                    File.WriteAllText("/tmp/error.glsl", comp.AllSources());
                throw new Exception($"Error occurred whilst compiling Shader({shader}): {msg}");
            }

            ObjectManager.SetLabel(comp);
        }

        private static void LinkProgram(int program)
        {
            // We link the program
            GL.LinkProgram(program);

            // Check for linking errors
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                var msg = GL.GetProgramInfoLog(program);
                Console.WriteLine(msg);

                throw new Exception($"Error occurred whilst linking Program({program}): {msg}");
            }
        }

        internal static int CurrentHandle { get; private set; }

        // A wrapper function that enables the shader program.
        public void Bind()
        {
            if (CurrentHandle == Handle)
                return;
            CurrentHandle = Handle;
            GL.UseProgram(Handle);
        }

        public void Free()
        {
            GL.DeleteProgram(Handle);
        }

        // The shader sources provided with this project use hardcoded layout(location)-s. If you want to do it dynamically,
        // you can omit the layout(location=X) lines in the vertex shader, and use this in VertexAttribPointer instead of the hardcoded values.
        public int GetAttribLocation(string attribName)
        {
            Bind();
            var attrHandle = GL.GetAttribLocation(Handle, attribName);
            if (attrHandle < 0)
            {
                Console.WriteLine($"GetAttribLocation({attribName}): attrib not found");
            }
            return attrHandle;
        }

        // Uniform setters
        // Uniforms are variables that can be set by user code, instead of reading them from the VBO.
        // You use VBOs for vertex-related data, and uniforms for almost everything else.

        // Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
        //     1. Bind the program you want to set the uniform on
        //     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
        //     3. Use the appropriate GL.Uniform* function to set the uniform.

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            if (_uniformLocations.TryGetValue(name, out var location))
            {
                Bind();
                GL.Uniform1(location, data);
            }
        }

        public void SetMaterial(string name, Material material)
        {
            material.WriteToShader(name, this);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            if (_uniformLocations.TryGetValue(name, out var location))
            {
                Bind();
                GL.Uniform1(location, data);
            }
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix4(string name, Matrix4 data)
        {
            if (_uniformLocations.TryGetValue(name, out var location))
            {
                Bind();
                GL.UniformMatrix4(location, true, ref data);
            }
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            if (_uniformLocations.TryGetValue(name, out var location))
            {
                Bind();
                GL.Uniform3(location, data);
            }
        }

        public void BindBlock(string blockName, BindingPoint bindingPoint)
        {
            if (_uniformBlockLocations.TryGetValue(blockName, out var location))
            {
                GL.UniformBlockBinding(Handle, location, bindingPoint.Number);
            }
        }

    }

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
                    if (entry.Value != null)
                    {
                        defineLine += " " + GetDefineLiteral(entry.Value);
                    }
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

            var absPath = DirectoryHelper.GetPath(path);
            if (string.IsNullOrEmpty(absPath))
                throw new Exception("Could not load file: " + path);

            using (var sr = new StreamReader(absPath, Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

    }

}