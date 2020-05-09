// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenToolkit.Graphics.OpenGL4;

namespace Aximo.Render
{

    public class BindingPoint
    {
        private int _Number;
        public int Number => _Number;

        private static SlotAllocator<int> Allocator;
        static BindingPoint()
        {
            var freeNumbers = new List<int>();
            Allocator = new SlotAllocator<int>(Enumerable.Range(1, 15), nameof(BindingPoint));
        }

        public static BindingPoint Default { get; private set; } = new BindingPoint(false) { _Number = 0 };

        public BindingPoint() : this(true)
        {
        }

        private BindingPoint(bool alloc)
        {
            if (alloc)
            {
                _Number = Allocator.Alloc(); ;
            }
        }

        public void Free()
        {
            Allocator.Free(_Number);
            _Number = -1;
        }
    }
}
