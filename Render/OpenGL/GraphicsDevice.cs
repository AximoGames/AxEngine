﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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
            Default.ReadState();
        }

        public static GraphicsDevice Default { get; private set; }

        private void SetCap(EnableCap cap, bool value)
        {
            if (value)
                GL.Enable(cap);
            else
                GL.Disable(cap);
        }

        public void ReadState()
        {
            _ScissorTest = GL.IsEnabled(EnableCap.ScissorTest);
            _Blend = GL.IsEnabled(EnableCap.Blend);
            _CullFace = GL.IsEnabled(EnableCap.CullFace);
            _CullFaceMode = (CullFaceMode)GL.GetInteger(GetPName.CullFaceMode);

            _DepthMask = GL.GetBoolean(GetPName.DepthWritemask);
            _DepthTest = GL.IsEnabled(EnableCap.DepthTest);
            _DepthFunc = (DepthFunction)GL.GetInteger(GetPName.DepthFunc);

            _ReadFramebuffer = GL.GetInteger(GetPName.ReadFramebufferBinding);
            _DrawFramebuffer = GL.GetInteger(GetPName.DrawFramebufferBinding);
            _Framebuffer = GL.GetInteger(GetPName.FramebufferBinding);
            _FramebufferExt = GL.GetInteger(GetPName.FramebufferBindingExt);

            _Program = GL.GetInteger(GetPName.CurrentProgram);
        }

        public void WriteState()
        {
            SetCap(EnableCap.ScissorTest, _ScissorTest);
            SetCap(EnableCap.Blend, _Blend);
            SetCap(EnableCap.CullFace, _CullFace);
            GL.CullFace((OpenToolkit.Graphics.OpenGL4.CullFaceMode)_CullFaceMode);

            GL.DepthMask(_DepthMask);
            SetCap(EnableCap.DepthTest, _DepthTest);
            GL.DepthFunc((OpenToolkit.Graphics.OpenGL4.DepthFunction)_DepthFunc);

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, _ReadFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _DrawFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _Framebuffer);
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, _FramebufferExt);

            GL.UseProgram(_Program);
        }

        private bool _ScissorTest;
        public bool ScissorTest
        {
            get => _ScissorTest;
            set
            {
                if (_ScissorTest == value)
                    return;
                SetCap(EnableCap.ScissorTest, value);
                _ScissorTest = value;
            }
        }

        private int _Program;
        public int Program
        {
            get => _Program;
            set
            {
                if (_Program == value)
                    return;
                GL.UseProgram(value);
                _Program = value;
            }
        }

        private int _FramebufferExt;
        public int FramebufferExt
        {
            get => _FramebufferExt;
            set
            {
                if (_FramebufferExt == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, value);
                _FramebufferExt = value;
            }
        }

        private int _Framebuffer;
        public int Framebuffer
        {
            get => _Framebuffer;
            set
            {
                if (_Framebuffer == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, value);
                _Framebuffer = value;
            }
        }

        private int _DrawFramebuffer;
        public int DrawFramebuffer
        {
            get => _DrawFramebuffer;
            set
            {
                if (_DrawFramebuffer == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, value);
                _DrawFramebuffer = value;
            }
        }

        private int _ReadFramebuffer;
        public int ReadFramebuffer
        {
            get => _ReadFramebuffer;
            set
            {
                if (_ReadFramebuffer == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, value);
                _ReadFramebuffer = value;
            }
        }

        private bool _CullFace;
        public bool CullFace
        {
            get => _CullFace;
            set
            {
                if (_CullFace == value)
                    return;
                SetCap(EnableCap.CullFace, value);
                _CullFace = value;
            }
        }

        private bool _Blend;
        public bool Blend
        {
            get => _Blend;
            set
            {
                if (_Blend == value)
                    return;
                SetCap(EnableCap.Blend, value);
                _Blend = value;
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

        private bool _DepthTest;
        public bool DepthTest
        {
            get => _DepthTest;
            set
            {
                if (_DepthTest == value)
                    return;
                SetCap(EnableCap.DepthTest, value);
                _DepthTest = value;
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
