using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace AxEngine
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
