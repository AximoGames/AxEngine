// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render.OpenGL
{

    public class GraphicsDeviceState
    {
        internal GraphicsDeviceState()
        {
        }

        internal bool ScissorTest;
        internal int Program;
        internal int FramebufferExt;
        internal int Framebuffer;
        internal int DrawFramebuffer;
        internal int ReadFramebuffer;
        internal bool CullFace;
        internal bool Blend;
        internal CullFaceMode CullFaceMode;
        internal bool DepthTest;
        internal bool DepthMask;
        internal DepthFunction DepthFunc;
        internal FrontFaceDirection FrontFace;
        internal bool VertexProgramPointSize;
        internal int VertexArrayBinding;

        public GraphicsDeviceState Clone()
        {
            return (GraphicsDeviceState)MemberwiseClone();
        }

    }

}
