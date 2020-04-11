﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
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

    public class BindingPoint
    {

        private int _Number;
        public int Number => _Number;

        private static HashSet<int> UsedNumbers = new HashSet<int>();
        private static List<int> FreeNumbers;

        static BindingPoint()
        {
            FreeNumbers = new List<int>();
            for (var i = 1; i < 16; i++)
            {
                FreeNumbers.Add(i);
            }
        }

        public static BindingPoint Default { get; private set; } = new BindingPoint(false) { _Number = 0 };

        public BindingPoint() : this(true)
        {
        }

        private BindingPoint(bool alloc)
        {
            if (alloc)
            {
                _Number = FreeNumbers[FreeNumbers.Count - 1];
                FreeNumbers.Remove(_Number);
                UsedNumbers.Add(_Number);
            }
        }

        public void Free()
        {
            UsedNumbers.Remove(_Number);
            FreeNumbers.Add(_Number);
            _Number = -1;
        }
    }

}