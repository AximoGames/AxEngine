﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using Aximo.Render;
using Aximo.Render.OpenGL;
using Aximo.Render.Pipelines;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public class Material : SceneObject
    {
        public static Material Default => MaterialManager.DefaultMaterial;

        private static int LastMaterialId = 0;
        public int MaterialId { get; private set; }
        internal RendererMaterial RendererMaterial;

        private List<Texture> Textures = new List<Texture>();

        public override void Visit<T>(Action<T> action, Func<T, bool> visitChilds = null)
        {
            base.Visit(action, visitChilds);

            if (visitChilds != null)
                if (this is T obj)
                    if (!visitChilds.Invoke(obj))
                        return;

            foreach (var txt in Textures)
                txt.Visit(action, visitChilds);
        }

        public void AddTexture(Texture txt)
        {
            Textures.Add(txt);
            txt.AddRef(this);
        }

        public void RemoveTexture(Texture txt)
        {
            Textures.Remove(txt);
            txt.RemoveRef(txt);
        }

        private Texture _DiffuseTexture;
        public Texture DiffuseTexture
        {
            get
            {
                return _DiffuseTexture;
            }
            set
            {
                if (_DiffuseTexture == value)
                    return;
                if (_DiffuseTexture != null)
                    RemoveTexture(_DiffuseTexture);

                _DiffuseTexture = value;
                AddTexture(value);
            }
        }

        private Texture _SpecularTexture;
        public Texture SpecularTexture
        {
            get
            {
                return _SpecularTexture;
            }
            set
            {
                if (_SpecularTexture == value)
                    return;
                if (_SpecularTexture != null)
                    RemoveTexture(_SpecularTexture);

                _SpecularTexture = value;
                AddTexture(value);
            }
        }

        // private Shader Shader;
        // private Shader DefGeometryShader;
        // private Shader ShadowShader;
        // private Shader CubeShadowShader;

        public Shader Shader { get; set; }
        public Shader DefGeometryShader { get; set; }
        public Shader ShadowShader { get; set; }
        public Shader CubeShadowShader { get; set; }

        public Material()
        {
            MaterialId = Interlocked.Increment(ref LastMaterialId);
        }

        public Vector4 Color { get; set; } = Vector4.One;
        public float Ambient { get; set; }
        public float Shininess { get; set; } = 1.0f;
        public float SpecularStrength { get; set; }
        public bool CastShadow { get; set; }
        public bool ReceiveShadow { get; set; } = true;
        public bool UseVertexColor { get; set; }

        private Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();
        private Dictionary<string, object> Defines = new Dictionary<string, object>();

        /// <summary>
        /// Specifies what pipeline should be used for this material.
        /// </summary>
        public PipelineType PipelineType { get; set; }
        public bool UseTransparency { get; set; }

        public void SetDefine(string name, string value)
        {
            Defines.Add(name, value);
        }

        public void AddParameter(string name, Matrix3 value)
        {
            AddParameter(new Parameter(name, value, ParamterType.Matrix3));
        }

        public void AddParameter(string name, Matrix4 value)
        {
            AddParameter(new Parameter(name, value, ParamterType.Matrix4));
        }

        public void AddParameter(string name, Texture value)
        {
            AddParameter(new Parameter(name, value, ParamterType.Texture));
        }

        public void AddParameter(string name, float value)
        {
            AddParameter(new Parameter(name, value, ParamterType.Float));
        }

        public void AddParameter(string name, bool value)
        {
            AddParameter(new Parameter(name, value, ParamterType.Bool));
        }

        public void AddParameter(string name, int value)
        {
            AddParameter(new Parameter(name, value, ParamterType.Int));
        }

        public void AddParameter(string name, Vector2 value)
        {
            AddParameter(new Parameter(name, value, ParamterType.Vector2));
        }

        public void AddParameter(string name, Vector3 value)
        {
            AddParameter(new Parameter(name, value, ParamterType.Vector3));
        }

        public void AddParameter(string name, Vector4 value)
        {
            AddParameter(new Parameter(name, value, ParamterType.Vector4));
        }

        public void AddParameter(Parameter parameter)
        {
            Parameter param;
            if (Parameters.TryGetValue(parameter.Name, out param))
            {
                if (Equals(parameter.Value, param.Value))
                    return;

                if (parameter.Type == param.Type)
                    return;

                param.Value = parameter.Value;
                param.HasChanges = true;
            }
            else
            {
                parameter.HasChanges = true;
                Parameters.Add(parameter.Name, parameter);
            }
        }

        internal virtual void PropagateChanges()
        {
        }

        internal virtual void SyncChanges()
        {
            // if (Shader == null)
            //     Shader = new Shader("Shaders/forward.vert", "Shaders/forward.frag");
            // if (DefGeometryShader == null)
            //     DefGeometryShader = new Shader("Shaders/deferred-gbuffer.vert", "Shaders/deferred-gbuffer.frag");
            // if (ShadowShader == null)
            //     ShadowShader = new Shader("Shaders/shadow-directional.vert", "Shaders/shadow-directional.frag", "Shaders/shadow-directional.geom");
            // if (CubeShadowShader == null)
            //     CubeShadowShader = new Shader("Shaders/shadow-cube.vert", "Shaders/shadow-cube.frag", "Shaders/shadow-cube.geom");

            DiffuseTexture?.Sync();
            SpecularTexture?.Sync();

            if (RendererMaterial == null)
            {
                RendererMaterial = new RendererMaterial();

                if (Shader == null)
                    Shader = new Shader("Shaders/forward.vert", "Shaders/forward.frag");
                if (DefGeometryShader == null)
                    DefGeometryShader = new Shader("Shaders/deferred-gbuffer.vert", "Shaders/deferred-gbuffer.frag");

                if (DiffuseTexture != null || SpecularTexture != null)
                    Defines.Add("USE_VERTEX_UV", "1");
                if (UseVertexColor)
                    Defines.Add("USE_VERTEX_COLOR", "1");
                if (ReceiveShadow && Renderer.Current.UseShadows)
                    Defines.Add("USE_SHADOW", "1");

                RendererMaterial.Shader = new RendererShader(Shader.VertexShaderPath, Shader.FragmentShaderPath, Shader.GeometryShaderPath, true, Defines);
                RendererMaterial.DefGeometryShader = new RendererShader(DefGeometryShader.VertexShaderPath, DefGeometryShader.FragmentShaderPath, DefGeometryShader.GeometryShaderPath, true, Defines);

                RendererMaterial.CreateShaders();
            }

            var mat = RendererMaterial;
            if (DiffuseTexture == null)
            {
                mat.DiffuseMap = InternalTextureManager.White;
                mat.DiffuseColor = Color;
            }
            else
            {
                mat.DiffuseMap = DiffuseTexture.RendererTexture;
                mat.DiffuseColor = Color;
            }

            if (SpecularTexture == null)
            {
                mat.SpecularMap = InternalTextureManager.White;
                mat.SpecularStrength = SpecularStrength;
            }
            else
            {
                mat.SpecularMap = SpecularTexture.RendererTexture;
                mat.SpecularStrength = 1.0f;
            }
            mat.CastShadow = CastShadow;

            mat.Ambient = Ambient;
            mat.Shininess = Shininess;
            mat.UseVertexColor = UseVertexColor;

            var pipelineType = PipelineType;
            if (pipelineType == PipelineType.Default)
                if (UseTransparency || !ReceiveShadow)
                    pipelineType = PipelineType.Forward;

            switch (pipelineType)
            {
                case PipelineType.Default:
                    mat.RenderPipeline = RenderContext.Current.PrimaryRenderPipeline;
                    break;
                case PipelineType.Forward:
                    mat.RenderPipeline = RenderContext.Current.GetPipeline<ForwardRenderPipeline>();
                    break;
                case PipelineType.Deferred:
                    mat.RenderPipeline = RenderContext.Current.GetPipeline<DeferredRenderPipeline>();
                    break;
                case PipelineType.Screen:
                    mat.RenderPipeline = RenderContext.Current.GetPipeline<ScreenPipeline>();
                    break;
            }

            foreach (var param in Parameters.Values)
            {
                if (param.HasChanges)
                {
                    param.HasChanges = false;
                    switch (param.Type)
                    {
                        case ParamterType.Bool:
                            mat.Shader.SetBool(param.Name, (bool)param.Value);
                            mat.DefGeometryShader.SetBool(param.Name, (bool)param.Value);
                            break;
                        case ParamterType.Int:
                            mat.Shader.SetInt(param.Name, (int)param.Value);
                            mat.DefGeometryShader.SetInt(param.Name, (int)param.Value);
                            break;
                        case ParamterType.Float:
                            mat.Shader.SetFloat(param.Name, (float)param.Value);
                            mat.DefGeometryShader.SetFloat(param.Name, (float)param.Value);
                            break;
                        case ParamterType.Vector2:
                            mat.Shader.SetVector2(param.Name, (Vector2)param.Value);
                            mat.DefGeometryShader.SetVector2(param.Name, (Vector2)param.Value);
                            break;
                        case ParamterType.Vector3:
                            mat.Shader.SetVector3(param.Name, (Vector3)param.Value);
                            mat.DefGeometryShader.SetVector3(param.Name, (Vector3)param.Value);
                            break;
                        case ParamterType.Vector4:
                            mat.Shader.SetVector4(param.Name, (Vector4)param.Value);
                            mat.DefGeometryShader.SetVector4(param.Name, (Vector4)param.Value);
                            break;
                        case ParamterType.Matrix4:
                            mat.Shader.SetMatrix4(param.Name, (Matrix4)param.Value);
                            mat.DefGeometryShader.SetMatrix4(param.Name, (Matrix4)param.Value);
                            break;
                    }
                }
            }
        }

        public class Parameter
        {
            public string Name;
            public object Value;
            public ParamterType Type;
            public bool HasChanges;

            public Parameter(string name, object value, ParamterType type)
            {
                Name = name;
                Value = value;
                Type = type;
            }
        }

        public enum ParamterType
        {
            Vector2,
            Vector3,
            Vector4,
            Matrix3,
            Matrix4,
            Float,
            Int,
            Bool,
            Texture,
        }
    }
}
