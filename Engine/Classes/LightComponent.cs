using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Aximo.Render;
using Aximo.Render.Objects;
using McMaster.NETCore.Plugins;
using OpenToolkit.Graphics.OpenGL;
using OpenToolkit.Mathematics;

namespace Aximo.Engine
{

    public abstract class LightComponent : Component
    {

        private LightObject? obj;

        public abstract LightType LightType { get; }

        private protected override void OnSyncRendererInternal()
        {
            base.OnSyncRendererInternal();
            if (obj == null)
            {
                obj = new LightObject();
                RenderContext.Current.AddObject(obj);
            }

            obj.LightType = LightType;
            obj.Name = ToString();
            obj.Position = Actor.Transform.Position;
        }

    }

}
