// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Aximo.VertexData;
using OpenToolkit;
using OpenToolkit.Mathematics;

namespace Aximo.Render.OpenGL
{
    public abstract class MeshData
    {
        public VertexLayoutDefinition Layout { get; protected set; }
        public abstract BufferData1D Data { get; }
        public virtual int VertexCount { get; protected set; }
        public BufferData1D<ushort> Indicies { get; protected set; }
        public virtual int IndiciesCount { get; protected set; }

        public VertexLayoutBinded BindLayoutToShader(RendererShader shader) => Layout.BindToShader(shader);
        public AxPrimitiveType PrimitiveType { get; protected set; }
    }
}
