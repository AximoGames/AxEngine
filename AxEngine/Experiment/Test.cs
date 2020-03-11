// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable CA1050 // Declare types in namespaces

namespace Aximo.Engine
{
    public class TestClass
    {
        public void Test()
        {
            ISceneInterface scene = null;
            Actor actor = null;
            var light = new PointLightComponent();
            actor.AddComponent(light);
            scene.AddLight(light);

            var box = new StaticMeshComponent();
            var mesh = new StaticMesh();
            var model = new StaticMeshSourceModel();
            mesh.SetSourceModel(model);
            box.SetMesh(mesh);
            scene.AddPrimitive(box);
        }
    }
}
