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

        internal bool _ScissorTest;
        internal int _Program;
        internal int _FramebufferExt;
        internal int _Framebuffer;
        internal int _DrawFramebuffer;
        internal int _ReadFramebuffer;
        internal bool _CullFace;
        internal bool _Blend;
        internal CullFaceMode _CullFaceMode;
        internal bool _DepthTest;
        internal bool _DepthMask;
        internal DepthFunction _DepthFunc;

        public GraphicsDeviceState Clone()
        {
            return (GraphicsDeviceState)MemberwiseClone();
        }

    }

}
