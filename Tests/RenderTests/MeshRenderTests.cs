// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Aximo.Engine;
using Aximo.Engine.Components.Geometry;
using Aximo.Engine.Components.Lights;
using Aximo.Render;
using OpenToolkit;
using OpenToolkit.Mathematics;
using Xunit;

namespace Aximo.AxTests
{
    public class MeshRenderTests : TestBase
    {
        public MeshRenderTests()
        {
        }

        [Fact]
        public void Sphere()
        {
            GameContext.AddActor(new Actor(new DirectionalLightComponent()
            {
                Name = "StaticLight",
                RelativeTranslation = new Vector3(-0.2f, -2.1f, 1.85f),
            }));

            GameMaterial material = new GameMaterial
            {
                DiffuseTexture = GameTexture.GetFromFile("Textures/woodenbox.png"),
                SpecularTexture = GameTexture.GetFromFile("Textures/woodenbox_specular.png"),
                Ambient = 0.5f,
                PipelineType = PipelineType.Deferred,
            };

            GameContext.AddActor(new Actor(new SphereComponent()
            {
                Name = "Sphere1",
                Transform = GetTestTransform(),
                Material = material,
            }));

            RenderAndCompare(nameof(Sphere));
        }

        [Fact]
        public void Bomb()
        {
            GameContext.AddActor(new Actor(new DirectionalLightComponent()
            {
                Name = "StaticLight",
                RelativeTranslation = new Vector3(-0.2f, -2.1f, 1.85f),
            }));

            var tmp = Mesh.CreateSphere();

            var m2 = Mesh.CreateCylinder();
            m2.Scale(0.3f, 0.3f);
            m2.Translate(new Vector3(0, 0, 0.05f));
            tmp.AddMesh(m2, 0, 1);

            var m3 = Mesh.CreateCylinder();
            m2.Scale(0.15f, 0.15f);
            m2.Translate(new Vector3(0, 0, 0.3f));
            tmp.AddMesh(m2, 0, 2);

            var comp = new StaticMeshComponent(tmp)
            {
                Name = "Bomb1",
                RelativeTranslation = new Vector3(0, 0, 0.55f),
                RelativeScale = new Vector3(2),
            };

            comp.AddMaterial(new GameMaterial()
            {
                Color = new Vector4(0.2f, 0.2f, 0.2f, 1),
                Ambient = 0.5f,
                Shininess = 64.0f,
                SpecularStrength = 1f,
                CastShadow = true,
            });

            comp.AddMaterial(new GameMaterial()
            {
                Color = new Vector4(0.1f, 0.1f, 0.1f, 1),
                Ambient = 0.5f,
                Shininess = 32.0f,
                SpecularStrength = 0.5f,
                CastShadow = true,
            });

            comp.AddMaterial(new GameMaterial()
            {
                Color = new Vector4(0.5f, 1 / 255f * 165 * 0.5f, 0, 1),
                Ambient = 0.5f,
                Shininess = 32.0f,
                SpecularStrength = 0.5f,
                CastShadow = true,
            });

            GameContext.AddActor(new Actor(comp));

            RenderAndCompare(nameof(Bomb));
        }
    }
}
