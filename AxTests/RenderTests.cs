// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Aximo.Engine;
using OpenTK;
using Xunit;

namespace Aximo.AxDemo
{

    public class RenderTests : RenderApplicationTests
    {

        public RenderTests() : base()
        {

        }

        [Fact]
        public void ForwardBox1()
        {
            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box2",
                Transform = GetTestTransform(),
                Material = GetTestMaterial(PipelineType.Forward, new Vector3(0, 1, 0)),
            }));
            RenderAndCompare(nameof(ForwardBox1));
        }

        [Fact]
        public void DeferredBox1()
        {
            GameContext.AddActor(new Actor(new DebugCubeComponent()
            {
                Name = "Box2",
                Transform = GetTestTransform(),
                Material = GetTestMaterial(PipelineType.Deferred, new Vector3(0, 1, 0)),
            }));
            RenderAndCompare(nameof(DeferredBox1));
        }

    }

}
