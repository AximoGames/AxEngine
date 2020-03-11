// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Aximo.Render;
using OpenTK;

namespace Aximo.Engine
{

    // Always called from Render Thread
    public class PrimitiveSceneProxy : IPrimitiveSceneProxy
    {
        public BoxSphereBounds Bounds { get; private set; }
        public void DrawStaticElements(IStaticPrimitiveDrawInterface meshInterface) { }
        public void GetDynamicMeshElements() { }
        public bool IsStatic { get; private set; }

        protected PrimitiveComponent PrimitiveComponent;

        public PrimitiveSceneProxy(PrimitiveComponent component)
        {
            PrimitiveComponent = component;
        }

    }

    public class StaticMeshSceneProxy : PrimitiveSceneProxy
    {
        protected StaticMeshComponent StaticMeshComponent => (StaticMeshComponent)PrimitiveComponent;
        public StaticMeshSceneProxy(StaticMeshComponent component) : base(component) { }

        public bool CastShadow { get; private set; }

    }

}
