// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
{
    public class VertexLayoutDefinitionAttribute
    {
        public string Name;
        public int Size;
        public VertexAttribPointerType Type;
        public bool Normalized;
        public int Stride;
        public int Offset;

        public void CopyTo(VertexLayoutDefinitionAttribute destination)
        {
            destination.Name = Name;
            destination.Size = Size;
            destination.Type = Type;
            destination.Normalized = Normalized;
            destination.Stride = Stride;
            destination.Offset = Offset;
        }

        internal virtual string GetDumpString()
        {
            return $"Name: {Name}, Size: {Size}, Type: {Type}, Normalized: {Normalized}, Stride: {Stride}, Offset: {Offset}";
        }

    }

}
