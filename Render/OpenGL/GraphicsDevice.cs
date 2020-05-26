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
        //
        // Summary:
        //     Original was GL_NEVER = 0x0200
        Never = 512,
        //
        // Summary:
        //     Original was GL_LESS = 0x0201
        Less = 513,
        //
        // Summary:
        //     Original was GL_EQUAL = 0x0202
        Equal = 514,
        //
        // Summary:
        //     Original was GL_LEQUAL = 0x0203
        Lequal = 515,
        //
        // Summary:
        //     Original was GL_GREATER = 0x0204
        Greater = 516,
        //
        // Summary:
        //     Original was GL_NOTEQUAL = 0x0205
        Notequal = 517,
        //
        // Summary:
        //     Original was GL_GEQUAL = 0x0206
        Gequal = 518,
        //
        // Summary:
        //     Original was GL_ALWAYS = 0x0207
        Always = 519
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
