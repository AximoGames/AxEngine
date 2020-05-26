// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenToolkit.Mathematics;

namespace Aximo.Engine
{
    public static class MaterialManager
    {
        public static Material _DefaultMaterial;
        public static Material DefaultMaterial
        {
            get
            {
                if (_DefaultMaterial == null)
                {
                    _DefaultMaterial = new Material
                    {
                        Color = new Vector4(0.5f, 0.5f, 0.5f, 1),
                        Ambient = 0.3f,
                        Shininess = 32.0f,
                        SpecularStrength = 0.5f,
                        CastShadow = true,
                    };
                }
                return _DefaultMaterial;
            }
        }

        public static Material DefaultLineMaterial { get; } = new Material
        {
            Shader = new Shader("Shaders/lines.vert", "Shaders/lines.frag"),
            PipelineType = PipelineType.Forward,
        };

        public static Material DefaultScreenMaterial { get; } = new Material
        {
            Shader = new Shader("Shaders/screen.vert", "Shaders/screen.frag"),
            PipelineType = PipelineType.Forward,
        };

        public static Material CreateScreenMaterial(string texturePath)
        {
            var mat = CreateScreenMaterial();
            mat.DiffuseTexture = Texture.GetFromFile(texturePath);
            return mat;
        }

        public static Material CreateScreenMaterial()
        {
            return new Material
            {
                Shader = new Shader("Shaders/screen.vert", "Shaders/screen.frag"),
                PipelineType = PipelineType.Screen,
            };
        }

        public static Material CreateNewMaterial()
        {
            return new Material();
        }
    }
}
