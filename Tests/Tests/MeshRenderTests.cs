// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Aximo.Engine;
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
        public void Box()
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

            RenderAndCompare(nameof(Box));
        }

    }
}
