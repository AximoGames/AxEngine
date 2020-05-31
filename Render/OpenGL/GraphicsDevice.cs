// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render.OpenGL
{
    public enum CullFaceMode
    {
        Front = 1028,
        Back = 1029,
        FrontAndBack = 1032,
    }

    public enum DepthFunction
    {
        Never = 512,
        Less = 513,
        Equal = 514,
        Lequal = 515,
        Greater = 516,
        Notequal = 517,
        Gequal = 518,
        Always = 519,
    }

    public class GraphicsDevice
    {
        static GraphicsDevice()
        {
            Default = new GraphicsDevice();
        }

        public static GraphicsDevice Default { get; private set; }

        private bool _CullFace;
        public bool CullFace
        {
            get => _CullFace;
            set
            {
                if (_CullFace == value)
                    return;
                if (value)
                    GL.Enable(EnableCap.CullFace);
                else
                    GL.Disable(EnableCap.CullFace);
                _CullFace = value;
            }
        }

        private CullFaceMode _CullFaceMode;
        public CullFaceMode CullFaceMode
        {
            get => _CullFaceMode;
            set
            {
                if (_CullFaceMode == value)
                    return;
                GL.CullFace((OpenToolkit.Graphics.OpenGL4.CullFaceMode)value);
                _CullFaceMode = value;
            }
        }

        private bool _DepthMask;
        public bool DepthMask
        {
            get => _DepthMask;
            set
            {
                if (_DepthMask == value)
                    return;
                GL.DepthMask(value);
                _DepthMask = value;
            }
        }

        public DepthFunction _DepthFunc;
        public DepthFunction DepthFunc
        {
            get => _DepthFunc;
            set
            {
                if (_DepthFunc == value)
                    return;
                GL.DepthFunc((OpenToolkit.Graphics.OpenGL4.DepthFunction)value);
                _DepthFunc = value;
            }
        }
    }
}
