using System;
using System.Collections.Generic;
using OpenTK;

namespace ProcEngine
{
    public class Material
    {
        public string DiffuseImagePath { get; set; }
        public string SpecularImagePath { get; set; }
        public Vector3 Color { get; set; }
        public float Ambient { get; set; }
        public float Shininess { get; set; }
        public float SpecularStrength { get; set; }

        public static Material GetDefault()
        {
            return new Material()
            {
                DiffuseImagePath = "Ressources/woodenbox.png",
                SpecularImagePath = "Ressources/woodenbox_specular.png",
                Color = new Vector3(1.0f, 1.0f, 0.0f),
                Ambient = 0.3f,
                Shininess = 32.0f,
                SpecularStrength = 0.5f,
            };
        }

        public void WriteToShader(string name, Shader shader)
        {
            var prefix = name += ".";
            shader.SetVector3(prefix + "color", Color);
            shader.SetInt(prefix + "diffuse", 0);
            shader.SetInt(prefix + "specular", 1);
            shader.SetFloat(prefix + "ambient", Ambient);
            shader.SetFloat(prefix + "shininess", Shininess);
            shader.SetFloat(prefix + "specularStrength", SpecularStrength);
        }
    }

}