// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public class GameMaterial
    {

        public static GameMaterial Default => MaterialManager.DefaultMaterial;

        private static int LastMaterialId = 0;
        public int MaterialId { get; private set; }
        internal Material InternalMaterial;

        public GameTexture DiffuseTexture;
        public GameTexture SpecularTexture;

        // private Shader Shader;
        // private Shader DefGeometryShader;
        // private Shader ShadowShader;
        // private Shader CubeShadowShader;

        public GameShader Shader { get; set; }
        public GameShader DefGeometryShader { get; set; }
        public GameShader ShadowShader { get; set; }
        public GameShader CubeShadowShader { get; set; }

        public GameMaterial()
        {
            MaterialId = Interlocked.Increment(ref LastMaterialId);
        }

        public Vector3 Color { get; set; }
        public float Ambient { get; set; }
        public float Shininess { get; set; } = 1.0f;
        public float SpecularStrength { get; set; }
        public bool CastShadow { get; set; }

        private Dictionary<string, Parameter> Parameters = new Dictionary<string, Parameter>();
        private Dictionary<string, object> Defines = new Dictionary<string, object>();

        public PipelineType PipelineType { get; set; }

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

        public void AddParameter(string name, GameTexture value)
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
                if (object.Equals(parameter.Value, param.Value))
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

            if (InternalMaterial == null)
            {
                InternalMaterial = new Material();

                if (Shader == null)
                    Shader = new GameShader("Shaders/forward.vert", "Shaders/forward.frag");
                if (DefGeometryShader == null)
                    DefGeometryShader = new GameShader("Shaders/deferred-gbuffer.vert", "Shaders/deferred-gbuffer.frag");

                InternalMaterial.Shader = new Shader(Shader.VertexShaderPath, Shader.FragmentShaderPath, Shader.GeometryShaderPath, true, Defines);
                InternalMaterial.DefGeometryShader = new Shader(DefGeometryShader.VertexShaderPath, DefGeometryShader.FragmentShaderPath, DefGeometryShader.GeometryShaderPath, true, Defines);

                InternalMaterial.CreateShaders();
            }

            var mat = InternalMaterial;
            if (DiffuseTexture == null)
            {
                mat.DiffuseMap = InternalTextureManager.White;
                mat.DiffuseColor = Color;
            }
            else
            {
                mat.DiffuseMap = DiffuseTexture.InternalTexture;
                mat.DiffuseColor = Vector3.One;
            }

            if (SpecularTexture == null)
            {
                mat.SpecularMap = InternalTextureManager.White;
                mat.SpecularStrength = SpecularStrength;
            }
            else
            {
                mat.SpecularMap = SpecularTexture.InternalTexture;
                mat.SpecularStrength = 1.0f;
            }
            mat.CastShadow = CastShadow;

            mat.Ambient = Ambient;
            mat.Shininess = Shininess;

            switch (PipelineType)
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
