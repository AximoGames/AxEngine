// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public Material()
        {
            _Textures = new List<Texture>();
            Textures = new ReadOnlyCollection<Texture>(_Textures);
        }

        public string DiffuseImagePath { get; set; }
        public string SpecularImagePath { get; set; }

        public Vector3 BaseColor { get; set; }
        public bool DisableDepthTest { get; set; }

        public Vector3 Color { get; set; }
        public float Ambient { get; set; }
        public float Shininess { get; set; }
        public float SpecularStrength { get; set; }
        public MaterialColorBlendMode ColorBlendMode { get; set; }

        public Vector3 WorldPositionOffset { get; set; }

        private List<Texture> _Textures;
        public ICollection<Texture> Textures { get; private set; }

        public void AddTexture(Texture texture)
        {
            _Textures.Add(texture);
        }

        public void RemoveTexture(Texture texture)
        {
            _Textures.Remove(texture);
        }

        public static Material GetDefault()
        {
            return new Material()
            {
                DiffuseImagePath = "Textures/woodenbox.png",
                SpecularImagePath = "Textures/woodenbox_specular.png",
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
            shader.SetInt(prefix + "colorBlendMode", (int)ColorBlendMode);
        }
    }

}