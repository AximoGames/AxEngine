// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
{
    public class UniformBufferObject : BufferObject
    {
        public UniformBufferObject()
        {
            Target = BufferTarget.UniformBuffer;
        }

        public void SetBindingPoint(BindingPoint bindingPoint)
        {
            if (Target != BufferTarget.UniformBuffer)
                throw new InvalidOperationException();

            GL.BindBufferBase(BufferRangeTarget.UniformBuffer, bindingPoint.Number, Handle);
        }
    }
}
