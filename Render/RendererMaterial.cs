// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Aximo.Render.OpenGL;
using Aximo.Render.Pipelines;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public enum MaterialColorBlendMode
    {
        None = 0,
        Set = 1,
        Multiply = 2,
        Add = 3,
        Sub = 4,
    }

    public class RendererMaterial
    {
        public Vector4 DiffuseColor { get; set; }

        public float SpecularStrength { get; set; }
        public float Shininess { get; set; }

        public float Ambient { get; set; }

        public bool CastShadow;

        public RendererShader Shader { get; set; }
        public RendererShader DefGeometryShader { get; set; }
        public RendererShader ShadowShader { get; set; }
        public RendererShader CubeShadowShader { get; set; }

        public RendererTexture DiffuseMap;
        public RendererTexture SpecularMap;

        public bool UseVertexColor;

        public IRenderPipeline RenderPipeline;

        public static RendererMaterial GetDefault()
        {
            var mat = new RendererMaterial()
            {
                DiffuseColor = new Vector4(0.5f, 0.5f, 0.5f, 1),
                Ambient = 0.3f,
                Shininess = 32.0f,
                SpecularStrength = 0.5f,
            };
            mat.CreateShaders();
            return mat;
        }

        public void CreateShaders()
        {
            if (Shader == null)
                Shader = new RendererShader("Shaders/forward.vert", "Shaders/forward.frag");
            if (DefGeometryShader == null)
                DefGeometryShader = new RendererShader("Shaders/deferred-gbuffer.vert", "Shaders/deferred-gbuffer.frag");
            if (ShadowShader == null)
                ShadowShader = new RendererShader("Shaders/shadow-directional.vert", "Shaders/shadow-directional.frag", "Shaders/shadow-directional.geom");
            if (CubeShadowShader == null)
                CubeShadowShader = new RendererShader("Shaders/shadow-cube.vert", "Shaders/shadow-cube.frag", "Shaders/shadow-cube.geom");
        }

        public void WriteToShader(string name, RendererShader shader)
        {
            var prefix = name += ".";
            shader.SetVector4(prefix + "DiffuseColor", DiffuseColor);
            shader.SetInt(prefix + "DiffuseMap", 0);
            shader.SetInt(prefix + "SpecularMap", 1);
            shader.SetFloat(prefix + "Ambient", Ambient);
            shader.SetFloat(prefix + "Shininess", Shininess);
            shader.SetFloat(prefix + "SpecularStrength", SpecularStrength);
        }
    }
}
