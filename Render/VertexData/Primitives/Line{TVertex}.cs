// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Aximo.Render.VertexData
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Line<TVertex> : IPrimitive<TVertex>
    {
        public TVertex Vertex0;
        public TVertex Vertex1;

        public Line(TVertex[] vertices)
        {
            Vertex0 = vertices[0];
            Vertex1 = vertices[1];
        }

        public Line(TVertex vertex0, TVertex vertex1)
        {
            Vertex0 = vertex0;
            Vertex1 = vertex1;
        }

        public TVertex this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Vertex0;
                    case 1:
                        return Vertex1;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        Vertex0 = value;
                        return;
                    case 1:
                        Vertex1 = value;
                        return;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public int Count => 2;

        public void CopyTo(ICollection<IPrimitive<TVertex>> destination)
        {
            for (var i = 0; i < Count; i++)
                destination.Add((IPrimitive<TVertex>)this[i]);
        }

        public void CopyTo(ICollection<IPrimitive> destination)
        {
            for (var i = 0; i < Count; i++)
                destination.Add((IPrimitive)this[i]);
        }

        public TVertex[] ToVertices()
        {
            return new TVertex[] { Vertex0, Vertex1 };
        }
    }
}
