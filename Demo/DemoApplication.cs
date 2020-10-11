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

            SceneManager.SetActiveScene(scene);

            for (var y = -10; y < 10; y++)
            {
                for (var x = -10; x < 10; x++)
                {
                    var act = new Actor();

                    scene.AddActor(act);
                    var m = act.AddComponent<MeshC>();
                    m.Mesh = Mesh.CreateCube();
                    act.Transform.Position = new Vector3(x, y, 0);
                }
            }

            //var comp = (ScriptBehaviour)act.AddComponent("TestClass");
            //var comp2 = (ScriptBehaviour)act.AddComponent("TestClass");
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
        }
    }
}
