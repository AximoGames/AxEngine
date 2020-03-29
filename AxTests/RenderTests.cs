// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Engine;
using OpenTK;
using Xunit;

namespace Aximo.AxTests
{

    public class RenderTests : RenderApplicationTests
    {

        public RenderTests()
        {

        }

        [Fact]
        public void BoxSolidColorForward()
        {
            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box1",
                Transform = GetTestTransform(),
                Material = SolidColorMaterial(PipelineType.Forward, new Vector3(0, 1, 0)),
            }));
            RenderAndCompare(nameof(BoxSolidColorForward));
        }

        [Fact]
        public void BoxSolidColorDeferred()
        {
            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box1",
                Transform = GetTestTransform(),
                Material = SolidColorMaterial(PipelineType.Deferred, new Vector3(0, 1, 0)),
            }));
            RenderAndCompare(nameof(BoxSolidColorDeferred));
        }

        [Fact]
        public void BoxSolidTextureForward()
        {
            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box1",
                Transform = GetTestTransform(),
                Material = SolidTextureMaterial(PipelineType.Forward),
            }));
            RenderAndCompare(nameof(BoxSolidTextureForward));
        }

        [Fact]
        public void BoxSolidTextureDeferred()
        {
            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box1",
                Transform = GetTestTransform(),
                Material = SolidTextureMaterial(PipelineType.Deferred),
            }));
            RenderAndCompare(nameof(BoxSolidTextureDeferred));
        }

    }

}
