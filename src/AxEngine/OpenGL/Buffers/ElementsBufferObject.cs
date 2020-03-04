using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace AxEngine
{
    public class ElementsBufferObject : BufferObject
    {

        public ElementsBufferObject()
        {
            Target = BufferTarget.ElementArrayBuffer;
        }

    }

}
