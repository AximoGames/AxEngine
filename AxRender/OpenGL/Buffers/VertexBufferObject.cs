// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using OpenTK.Graphics.OpenGL4;

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
