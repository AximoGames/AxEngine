// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.Loader;
using Aximo.Engine;
using Aximo.Engine.Components.Geometry;
using OpenToolkit.Mathematics;
using OpenToolkit.Windowing.Common;

namespace Aximo.AxDemo
{
    public class DemoApplication : Application
    {

        protected override void SetupScene()
        {
            var scene = new Scene();

            //new TestClass().Invoke();

            var act = new Actor();

            SceneManager.SetActiveScene(scene);

            scene.AddActor(act);
            var comp = (ScriptBehaviour)act.AddComponent("TestClass");
            var comp2 = (ScriptBehaviour)act.AddComponent("TestClass");
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
        }
    }
}
