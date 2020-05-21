// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render.OpenGL
{
    public abstract class BufferObject
    {
        public BufferObject()
        {
        }

        protected BufferTarget Target = BufferTarget.ArrayBuffer;

        private int _Handle;
        public int Handle => _Handle;

        public void Create()
        {
            _Handle = GL.GenBuffer();
        }

        public int Size { get; private set; }

        public unsafe void SetData(BufferData data)
        {
            var currentBuffer = CurrentBuffer;
            Bind();
            Size = data.Length;
            GCHandle h = data.CreateHandle();
            try
            {
                GL.BufferData(Target, data.Length * data.ElementSize, h.AddrOfPinnedObject(), BufferUsageHint.StaticDraw);
            }
            finally
            {
                h.Free();
            }
        }

        private static int CurrentHandle;
        private static BufferObject CurrentBuffer;

        public void Bind()
        {
            Bind(Target, _Handle);
            CurrentBuffer = this;
        }

        private static void Bind(BufferTarget target, int handle)
        {
            // if (CurrentHandle == handle)
            //     return;
            CurrentHandle = handle;
            GL.BindBuffer(target, handle);
        }

        public void BindDefault()
        {
            Bind(Target, 0);
            CurrentBuffer = null;
        }

        public void Free()
        {
            GL.DeleteBuffer(_Handle);
        }
    }
}
