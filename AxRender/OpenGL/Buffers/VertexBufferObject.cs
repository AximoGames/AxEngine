using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Aximo.Render
{
    public class VertexBufferObject : BufferObject
    {

        public VertexBufferObject()
        {
            Target = BufferTarget.ArrayBuffer;
        }

    }

}
