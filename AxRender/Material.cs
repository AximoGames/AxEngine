// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using OpenTK;

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

    public class Material
    {

        public string DiffuseImagePath { get; set; }
        public string SpecularImagePath { get; set; }
        public Vector3 Color { get; set; }
        public float Ambient { get; set; }
        public float Shininess { get; set; }
        public float SpecularStrength { get; set; }
        public MaterialColorBlendMode ColorBlendMode { get; set; }

        public bool CastShadow;

        public Shader Shader { get; set; }
        public Shader DefGeometryShader { get; set; }
        public Shader ShadowShader { get; set; }
        public Shader CubeShadowShader { get; set; }

        public Texture Txt0;
        public Texture Txt1;

        public IRenderPipeline RenderPipeline;

        public static Material GetDefault()
        {
            var mat = new Material()
            {
                DiffuseImagePath = "Textures/woodenbox.png",
                SpecularImagePath = "Textures/woodenbox_specular.png",
                Color = new Vector3(1.0f, 1.0f, 0.0f),
                Ambient = 0.3f,
                Shininess = 32.0f,
                SpecularStrength = 0.5f,
            };
            mat.CreateShaders();
            return mat;
        }

        public void CreateShaders()
        {
            if (Txt0 == null && !string.IsNullOrEmpty(DiffuseImagePath))
                Txt0 = new Texture(DiffuseImagePath);
            if (Txt1 == null && !string.IsNullOrEmpty(SpecularImagePath))
                Txt1 = new Texture(SpecularImagePath);

            if (Shader == null)
                Shader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            if (DefGeometryShader == null)
                DefGeometryShader = new Shader("Shaders/deferred-gbuffer.vert", "Shaders/deferred-gbuffer.frag");
            if (ShadowShader == null)
                ShadowShader = new Shader("Shaders/shadow-directional.vert", "Shaders/shadow-directional.frag", "Shaders/shadow-directional.geom");
            if (CubeShadowShader == null)
                CubeShadowShader = new Shader("Shaders/shadow-cube.vert", "Shaders/shadow-cube.frag", "Shaders/shadow-cube.geom");
        }

        public static Material Default { get; } = GetDefault();

        public void WriteToShader(string name, Shader shader)
        {
            var prefix = name += ".";
            shader.SetVector3(prefix + "Color", Color);
            shader.SetInt(prefix + "Diffuse", 0);
            shader.SetInt(prefix + "Specular", 1);
            shader.SetFloat(prefix + "Ambient", Ambient);
            shader.SetFloat(prefix + "Shininess", Shininess);
            shader.SetFloat(prefix + "SpecularStrength", SpecularStrength);
            shader.SetInt(prefix + "ColorBlendMode", (int)ColorBlendMode);
        }
    }

}
