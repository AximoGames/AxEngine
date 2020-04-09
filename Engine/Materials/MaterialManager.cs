// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public static class MaterialManager
    {

        public static GameMaterial _DefaultMaterial;
        public static GameMaterial DefaultMaterial
        {
            get
            {
                if (_DefaultMaterial == null)
                {
                    _DefaultMaterial = new GameMaterial
                    {
                        Color = new Vector3(0.5f, 0.5f, 0.5f),
                        Ambient = 0.3f,
                        Shininess = 32.0f,
                        SpecularStrength = 0.5f,
                        CastShadow = true,
                    };
                }
                return _DefaultMaterial;
            }
        }

        public static GameMaterial DefaultLineMaterial { get; } = new GameMaterial
        {
            Shader = new GameShader("Shaders/lines.vert", "Shaders/lines.frag"),
            PipelineType = PipelineType.Forward,
        };

        public static GameMaterial DefaultScreenMaterial { get; } = new GameMaterial
        {
            Shader = new GameShader("Shaders/screen.vert", "Shaders/screen.frag"),
            PipelineType = PipelineType.Forward,
        };

        public static GameMaterial CreateScreenMaterial(string texturePath)
        {
            var mat = CreateScreenMaterial();
            mat.DiffuseTexture = GameTexture.GetFromFile(texturePath);
            return mat;
        }

        public static GameMaterial CreateScreenMaterial()
        {
            return new GameMaterial
            {
                Shader = new GameShader("Shaders/screen.vert", "Shaders/screen.frag"),
                PipelineType = PipelineType.Screen,
            };
        }

        public static GameMaterial CreateNewMaterial()
        {
            return new GameMaterial();
        }

    }

}
