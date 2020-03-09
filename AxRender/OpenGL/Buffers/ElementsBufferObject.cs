using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;

namespace Aximo.Render
{
    public class ElementsBufferObject : BufferObject
    {

        public ElementsBufferObject()
        {
            Target = BufferTarget.ElementArrayBuffer;
        }

    }

}
