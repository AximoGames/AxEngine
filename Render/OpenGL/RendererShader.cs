// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aximo.Render.OpenGL
{

    // A simple class meant to help create shaders.
    public class RendererShader : IObjectLabel, IDisposable
    {

        public static void ReloadAll()
        {
            ShaderCache.Clear();
            //ShaderManager.ReloadAll();
        }

        private static Serilog.ILogger Log = Aximo.Log.ForContext<RendererShader>();

        public int Handle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get;
            private set;
        }
        public string ObjectLabel { get { return Compilations.FirstOrDefault()?.ObjectLabel ?? ""; } set { } }

        public ObjectLabelIdentifier ObjectLabelIdentifier => ObjectLabelIdentifier.Program;

        private Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();
        private Dictionary<string, int> _uniformBlockLocations = new Dictionary<string, int>();

        private List<ShaderCompilation> Compilations = new List<ShaderCompilation>();

        public string AttributeNameForField(string structFieldName)
        {
            switch (structFieldName)
            {
                case "Position":
                    return "aPos";
                case "Normal":
                    return "aNormal";
                case "UV":
                    return "aTexCoords";
                case "Color":
                    return "aColor";
                default:
                    return structFieldName;
            }
        }

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

        private RendererShader()
        {
        }

        // This is how you create a simple shader.
        // Shaders are written in GLSL, which is a language very similar to C in its semantics.
        // The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
        // A commented example of GLSL can be found in shader.vert
        public RendererShader(string vertPath, string fragPath, string geomPath = null, bool compile = true, IDictionary<string, object> defines = null)
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

            if (defines != null)
                foreach (var entry in defines)
                    SetDefine(entry.Key, entry.Value);

            if (compile)
                Compile();
        }

        ~RendererShader()
        {
            Dispose(false);
        }

        internal int SourceHash { get; set; }

        private static Dictionary<int, RendererShader> ShaderCache = new Dictionary<int, RendererShader>();
        private bool Cached;
        public void Compile()
        {
            // There are several different types of shaders, but the only two you need for basic rendering are the vertex and fragment shaders.
            // The vertex shader is responsible for moving around vertices, and uploading that data to the fragment shader.
            //   The vertex shader won't be too important here, but they'll be more important later.
            // The fragment shader is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.
            //   The fragment shader is what we'll be using the most here.

            // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
            foreach (var comp in Compilations)
                comp.GenerateSource();

            var hashSource = string.Join(' ', Compilations.SelectMany(comp => comp.Sources.Select(s => s.Source)));
            SourceHash = Hashing.FNV32(hashSource);
            if (ShaderCache.TryGetValue(SourceHash, out var shader))
            {
                SetFrom(shader);
                Cached = true;
                Bind();
                //Log.Warn("Multiple compilation of source: {ShaderLabel}. Use cached Shader.", ObjectLabel);
                return;
            }

            foreach (var comp in Compilations)
            {
                var shaderSources = comp.Sources.Select(s => s.Source).ToArray();
                var len = shaderSources.Select(s => s.Length).ToArray();

                comp.Handle = GL.CreateShader(comp.Type);
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

            int shaderSize;
            GL.GetProgram(Handle, (GetProgramParameterName)0x8741, out shaderSize);

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

                if (_uniformLocations.Count == 808)
                {
                    var s = "";
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

            ShaderCache.TryAdd(SourceHash, this);
            ShaderManager.Register(this);
            Log.Verbose("Compiled shader {ObjectLabel}, Size {Size}", ObjectLabel, shaderSize);
        }

        public void Reload()
        {
            try
            {
                var sh = new RendererShader();
                foreach (var comp in Compilations)
                {
                    var shComp = new ShaderCompilation
                    {
                        Type = comp.Type,
                    };

                    sh.Compilations.Add(shComp);

                    foreach (var src in comp.Sources)
                    {
                        shComp.Sources.Add(new ShaderSource
                        {
                            Compilation = shComp,
                            Ordinal = src.Ordinal,
                            Path = src.Path,
                        });
                    }

                    foreach (var entry in comp.Defines)
                        shComp.Defines.Add(entry.Key, entry.Value);
                }

                sh.Compile();
                SetFrom(sh);
                ShaderManager.Unregister(sh);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void SetFrom(RendererShader source)
        {
            Handle = source.Handle;
            SourceHash = source.SourceHash;
            Compilations = source.Compilations;
            SharedUniformValues = source.SharedUniformValues;
            LocalUniformValues = source.LocalUniformValues;

            _uniformLocations = source._uniformLocations;
            _uniformBlockLocations = source._uniformBlockLocations;
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
                File.WriteAllText(Path.Combine(AssetManager.GlobalCacheDir, "error.glsl"), comp.AllSources());
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

        internal static int CurrentHandle
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get;
            private set;
        }

        private static RendererShader CurrentShader;

        // A wrapper function that enables the shader program.
        public void Bind()
        {
            if (CurrentShader == this)
                return;
            CurrentShader = this;

            if (Cached)
            {
                var s = "";
            }
            BindInternal();
            BindParams();
        }

        public void BindInternal()
        {
            if (CurrentHandle == Handle)
                return;
            CurrentHandle = Handle;
            GL.UseProgram(Handle);
        }

        private void BindParams()
        {
            foreach (var kv in LocalUniformValues)
            {
                var location = kv.Key;
                var value = kv.Value;

                if (SharedUniformValues.TryGetValue(location, out var v))
                {
                    if (Equals(value, v))
                        continue;

                    SharedUniformValues[location] = value;
                }
                else
                {
                    SharedUniformValues.Add(location, value);
                }

                var type = value.GetType();
                if (type == typeof(int))
                    BindParam(location, (int)value, GL.Uniform1);
                else if (type == typeof(float))
                    BindParam(location, (float)value, GL.Uniform1);
                else if (type == typeof(bool))
                    BindParam(location, (bool)value ? 1 : 0, GL.Uniform1);
                else if (type == typeof(Vector2))
                    BindParam(location, (Vector2)value, GL.Uniform2);
                else if (type == typeof(Vector3))
                    BindParam(location, (Vector3)value, GL.Uniform3);
                else if (type == typeof(Vector4))
                    BindParam(location, (Vector4)value, GL.Uniform4);
                else if (type == typeof(Matrix3))
                    BindParam(location, (Matrix3)value, GLUniformMatrix3);
                else if (type == typeof(Matrix4))
                    BindParam(location, (Matrix4)value, GLUniformMatrix4);
                else
                    throw new ArgumentOutOfRangeException(type.Name);
            }
        }

        private void BindParam<T>(int location, T value, Action<int, T> setter)
        {
            SharedUniformValues.Set(location, value);
            setter(location, value);
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
                Log.Warn($"GetAttribLocation({attribName}): attrib not found");
            }
            return attrHandle;
        }

        private Dictionary<int, object> SharedUniformValues = new Dictionary<int, object>();
        private Dictionary<int, object> LocalUniformValues = new Dictionary<int, object>();
        private bool disposedValue;

        private bool SetInternal<T>(string name, T value, Action<int, T> setter)
        {
            if (!_uniformLocations.TryGetValue(name, out var location))
                return false;

            LocalUniformValues.Set(location, value);

            if (CurrentHandle != Handle)
                return false;

            if (SharedUniformValues.TryGetValue(location, out var v))
            {
                if (Equals(value, v))
                    return false;

                SharedUniformValues[location] = value;
            }
            else
            {
                SharedUniformValues.Add(location, value);
            }

            Bind();
            setter(location, value);

            return true;
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
        public void SetBool(string name, bool data) => SetInternal(name, data ? 1 : 0, GL.Uniform1);

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data) => SetInternal(name, data, GL.Uniform1);

        public void SetMaterial(string name, RendererMaterial material)
        {
            material.WriteToShader(name, this);
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data) => SetInternal(name, data, GL.Uniform1);

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
        public void SetMatrix4(string name, Matrix4 data) => SetInternal(name, data, GLUniformMatrix4);

        private static void GLUniformMatrix3(int location, Matrix3 data) => GL.UniformMatrix3(location, true, ref data);
        private static void GLUniformMatrix4(int location, Matrix4 data) => GL.UniformMatrix4(location, true, ref data);

        /// <summary>
        /// Set a uniform Matrix3 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix3(string name, Matrix3 data) => SetInternal(name, data, GLUniformMatrix3);

        /// <summary>
        /// Set a uniform Vector2 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector2(string name, Vector2 data) => SetInternal(name, data, GL.Uniform2);

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data) => SetInternal(name, data, GL.Uniform3);

        /// <summary>
        /// Set a uniform Vector4 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector4(string name, Vector4 data) => SetInternal(name, data, GL.Uniform4);

        public void BindBlock(string blockName, BindingPoint bindingPoint)
        {
            if (_uniformBlockLocations.TryGetValue(blockName, out var location))
            {
                GL.UniformBlockBinding(Handle, location, bindingPoint.Number);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            ShaderManager.Unregister(this);
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~RendererShader()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
