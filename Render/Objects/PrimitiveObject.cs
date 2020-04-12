// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using OpenToolkit;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit.Mathematics;

namespace Aximo.Render
{
    public abstract class PrimitiveObject : RenderObject, IRenderableObject
    {
        public void OnRender()
        {
            if (Context.CurrentPipeline is ForwardRenderPipeline && this is IForwardRenderable m1)
                m1.OnForwardRender();
            else if (Context.CurrentPipeline is DeferredRenderPipeline && this is IDeferredRenderable m2)
                m2.OnDeferredRender();
            else if (Context.CurrentPipeline is DirectionalShadowRenderPipeline && this is IShadowObject m3)
                m3.OnRenderShadow();
            else if (Context.CurrentPipeline is PointShadowRenderPipeline && this is IShadowObject m4)
                m4.OnRenderCubeShadow();
            else if (Context.CurrentPipeline is ScreenPipeline && this is IScreenRenderable m5)
                m5.OnScreenRender();
        }

        protected Dictionary<string, object> ShaderParams = new Dictionary<string, object>();

        public void AddShaderParam<T>(string name, T value)
            where T : struct
        {
            if (ShaderParams.ContainsKey(name))
                ShaderParams[name] = value;
            else
                ShaderParams.Add(name, value);
        }

        protected void ApplyShaderParams(Shader shader)
        {
            foreach (var entry in ShaderParams)
            {
                var name = entry.Key;
                var value = entry.Value;
                if (value == null)
                    continue;

                var type = value.GetType();
                if (type == typeof(int))
                    shader.SetInt(name, (int)value);
                else if (type == typeof(Vector3))
                    shader.SetVector3(name, (Vector3)value);
                else
                    throw new NotSupportedException(type.Name);
            }
        }
    }
}
