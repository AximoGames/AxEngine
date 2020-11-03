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
            s.ScissorTest = GL.IsEnabled(EnableCap.ScissorTest);
            s.Blend = GL.IsEnabled(EnableCap.Blend);
            s.CullFace = GL.IsEnabled(EnableCap.CullFace);
            s.CullFaceMode = (CullFaceMode)GL.GetInteger(GetPName.CullFaceMode);

            s.DepthMask = GL.GetBoolean(GetPName.DepthWritemask);
            s.DepthTest = GL.IsEnabled(EnableCap.DepthTest);
            s.DepthFunc = (DepthFunction)GL.GetInteger(GetPName.DepthFunc);

            s.ReadFramebuffer = GL.GetInteger(GetPName.ReadFramebufferBinding);
            s.DrawFramebuffer = GL.GetInteger(GetPName.DrawFramebufferBinding);
            s.Framebuffer = GL.GetInteger(GetPName.FramebufferBinding);
            s.FramebufferExt = GL.GetInteger(GetPName.FramebufferBindingExt);

            s.Program = GL.GetInteger(GetPName.CurrentProgram);
        }

        internal void WriteStateToDevice()
        {
            WriteStateToDevice(State);
        }

        internal void WriteStateToDevice(GraphicsDeviceState s)
        {
            SetCap(EnableCap.ScissorTest, s.ScissorTest);
            SetCap(EnableCap.Blend, s.Blend);
            SetCap(EnableCap.CullFace, s.CullFace);
            GL.CullFace((OpenToolkit.Graphics.OpenGL4.CullFaceMode)s.CullFaceMode);

            GL.DepthMask(s.DepthMask);
            SetCap(EnableCap.DepthTest, s.DepthTest);
            GL.DepthFunc((OpenToolkit.Graphics.OpenGL4.DepthFunction)s.DepthFunc);

            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, s.ReadFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, s.DrawFramebuffer);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, s.Framebuffer);
            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, s.FramebufferExt);

            GL.UseProgram(s.Program);
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
            get => State.ScissorTest;
            set
            {
                var state = State;
                if (state.ScissorTest == value)
                    return;
                SetCap(EnableCap.ScissorTest, value);
                state.ScissorTest = value;
            }
        }

        public int Program
        {
            get => State.Program;
            set
            {
                var state = State;
                if (state.Program == value)
                    return;
                GL.UseProgram(value);
                state.Program = value;
            }
        }

        public int FramebufferExt
        {
            get => State.FramebufferExt;
            set
            {
                var state = State;
                if (state.FramebufferExt == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.FramebufferExt, value);
                state.FramebufferExt = value;
            }
        }

        public int Framebuffer
        {
            get => State.Framebuffer;
            set
            {
                var state = State;
                if (state.Framebuffer == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, value);
                state.Framebuffer = value;
            }
        }

        public int DrawFramebuffer
        {
            get => State.DrawFramebuffer;
            set
            {
                var state = State;
                if (state.DrawFramebuffer == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, value);
                state.DrawFramebuffer = value;
            }
        }

        public int ReadFramebuffer
        {
            get => State.ReadFramebuffer;
            set
            {
                var state = State;
                if (state.ReadFramebuffer == value)
                    return;
                GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, value);
                state.ReadFramebuffer = value;
            }
        }

        public bool CullFace
        {
            get => State.CullFace;
            set
            {
                var state = State;
                if (state.CullFace == value)
                    return;
                SetCap(EnableCap.CullFace, value);
                state.CullFace = value;
            }
        }

        public bool Blend
        {
            get => State.Blend;
            set
            {
                var state = State;
                if (state.Blend == value)
                    return;
                SetCap(EnableCap.Blend, value);
                state.Blend = value;
            }
        }

        public CullFaceMode CullFaceMode
        {
            get => State.CullFaceMode;
            set
            {
                var state = State;
                if (state.CullFaceMode == value)
                    return;
                GL.CullFace((OpenToolkit.Graphics.OpenGL4.CullFaceMode)value);
                state.CullFaceMode = value;
            }
        }

        public bool DepthTest
        {
            get => State.DepthTest;
            set
            {
                var state = State;
                if (state.DepthTest == value)
                    return;
                SetCap(EnableCap.DepthTest, value);
                state.DepthTest = value;
            }
        }

        public bool DepthMask
        {
            get => State.DepthMask;
            set
            {
                var state = State;
                if (state.DepthMask == value)
                    return;
                GL.DepthMask(value);
                state.DepthMask = value;
            }
        }

        public DepthFunction DepthFunc
        {
            get => State.DepthFunc;
            set
            {
                var state = State;
                if (state.DepthFunc == value)
                    return;
                GL.DepthFunc((OpenToolkit.Graphics.OpenGL4.DepthFunction)value);
                state.DepthFunc = value;
            }
        }
    }
}
