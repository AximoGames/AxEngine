// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
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
            Default.ReadStateFromDevice();
        }

        public void SetState(GraphicsDeviceState state)
        {
            State = state;
            WriteStateToDevice(state);
        }

        public void ReadStateFromDevice()
        {
            ReadStateFromDevice(State);
        }

        internal void ReadStateFromDevice(GraphicsDeviceState s)
        {
            s._ScissorTest = GL.IsEnabled(EnableCap.ScissorTest);
            s._Blend = GL.IsEnabled(EnableCap.Blend);
            s._CullFace = GL.IsEnabled(EnableCap.CullFace);
            s._CullFaceMode = (CullFaceMode)GL.GetInteger(GetPName.CullFaceMode);

            s._DepthMask = GL.GetBoolean(GetPName.DepthWritemask);
            s._DepthTest = GL.IsEnabled(EnableCap.DepthTest);
            s._DepthFunc = (DepthFunction)GL.GetInteger(GetPName.DepthFunc);

            s._ReadFramebuffer = GL.GetInteger(GetPName.ReadFramebufferBinding);
            s._DrawFramebuffer = GL.GetInteger(GetPName.DrawFramebufferBinding);
            s._Framebuffer = GL.GetInteger(GetPName.FramebufferBinding);
            s._FramebufferExt = GL.GetInteger(GetPName.FramebufferBindingExt);

            s._Program = GL.GetInteger(GetPName.CurrentProgram);
        }

        internal void WriteStateToDevice()
        {
            WriteStateToDevice(State);
        }

        internal void WriteStateToDevice(GraphicsDeviceState s)
        {
            SetCap(EnableCap.ScissorTest, s._ScissorTest);
            SetCap(EnableCap.Blend, s._Blend);
            SetCap(EnableCap.CullFace, s._CullFace);
            GL.CullFace((OpenToolkit.Graphics.OpenGL4.CullFaceMode)s._CullFaceMode);

            GL.DepthMask(s._DepthMask);
            SetCap(EnableCap.DepthTest, s._DepthTest);
            GL.DepthFunc((OpenToolkit.Graphics.OpenGL4.DepthFunction)s._DepthFunc);

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, s._ReadFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, s._DrawFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, s._Framebuffer);
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, s._FramebufferExt);

            GL.UseProgram(s._Program);
        }

        public GraphicsDeviceState State = new GraphicsDeviceState();

        public static GraphicsDevice Default { get; private set; }

        private void SetCap(EnableCap cap, bool value)
        {
            if (value)
                GL.Enable(cap);
            else
                GL.Disable(cap);
        }

        public bool ScissorTest
        {
            get => State._ScissorTest;
            set
            {
                if (State._ScissorTest == value)
                    return;
                SetCap(EnableCap.ScissorTest, value);
                State._ScissorTest = value;
            }
        }

        public int Program
        {
            get => State._Program;
            set
            {
                if (State._Program == value)
                    return;
                GL.UseProgram(value);
                State._Program = value;
            }
        }

        public int FramebufferExt
        {
            get => State._FramebufferExt;
            set
            {
                if (State._FramebufferExt == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, value);
                State._FramebufferExt = value;
            }
        }

        public int Framebuffer
        {
            get => State._Framebuffer;
            set
            {
                if (State._Framebuffer == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, value);
                State._Framebuffer = value;
            }
        }

        public int DrawFramebuffer
        {
            get => State._DrawFramebuffer;
            set
            {
                if (State._DrawFramebuffer == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, value);
                State._DrawFramebuffer = value;
            }
        }

        public int ReadFramebuffer
        {
            get => State._ReadFramebuffer;
            set
            {
                if (State._ReadFramebuffer == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, value);
                State._ReadFramebuffer = value;
            }
        }

        public bool CullFace
        {
            get => State._CullFace;
            set
            {
                if (State._CullFace == value)
                    return;
                SetCap(EnableCap.CullFace, value);
                State._CullFace = value;
            }
        }

        public bool Blend
        {
            get => State._Blend;
            set
            {
                if (State._Blend == value)
                    return;
                SetCap(EnableCap.Blend, value);
                State._Blend = value;
            }
        }

        public CullFaceMode CullFaceMode
        {
            get => State._CullFaceMode;
            set
            {
                if (State._CullFaceMode == value)
                    return;
                GL.CullFace((OpenToolkit.Graphics.OpenGL4.CullFaceMode)value);
                State._CullFaceMode = value;
            }
        }

        public bool DepthTest
        {
            get => State._DepthTest;
            set
            {
                if (State._DepthTest == value)
                    return;
                SetCap(EnableCap.DepthTest, value);
                State._DepthTest = value;
            }
        }

        public bool DepthMask
        {
            get => State._DepthMask;
            set
            {
                if (State._DepthMask == value)
                    return;
                GL.DepthMask(value);
                State._DepthMask = value;
            }
        }

        public DepthFunction DepthFunc
        {
            get => State._DepthFunc;
            set
            {
                if (State._DepthFunc == value)
                    return;
                GL.DepthFunc((OpenToolkit.Graphics.OpenGL4.DepthFunction)value);
                State._DepthFunc = value;
            }
        }
    }
}
