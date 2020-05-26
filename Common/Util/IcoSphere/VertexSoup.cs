// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Adapted from this excellent IcoSphere tutorial by Andreas Kahler
// http://blog.andreaskahler.com/2009/06/creating-icosphere-mesh-in-code.html
// Changes Copyright (C) 2014 by David Jeske, and donated to the public domain.

using System.Collections.Generic;

namespace Aximo.Util.IcoSphere
{
    public class VertexSoup<TVertexStruct>
    {
        private Dictionary<TVertexStruct, ushort> vertexToIndexMap = new Dictionary<TVertexStruct, ushort>();
        public List<TVertexStruct> Verticies = new List<TVertexStruct>();
        private readonly bool deDup;

        public ushort DigestVertex(ref TVertexStruct vertex)
        {
            ushort retval;
            if (deDup && vertexToIndexMap.ContainsKey(vertex))
            {
                retval = vertexToIndexMap[vertex];
            }
            else
            {
                ushort nextIndex = (ushort)Verticies.Count;
                vertexToIndexMap[vertex] = nextIndex;
                Verticies.Add(vertex);
                retval = nextIndex;
            }

            return retval;
        }

        public ushort[] DigestVerticies(TVertexStruct[] vertex_list)
        {
            ushort[] retval = new ushort[vertex_list.Length];

            for (int x = 0; x < vertex_list.Length; x++)
            {
                retval[x] = DigestVertex(ref vertex_list[x]);
            }
            return retval;
        }

        public VertexSoup(bool deDup = true)
        {
            this.deDup = deDup;
        }
    }
}
