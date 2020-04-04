// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using OpenToolkit.Mathematics;
using System;

namespace Aximo.Render
{
    public static class InternalTextureManager
    {
        public static Texture White { get; private set; }
        public static Texture Black { get; private set; }
        public static Texture Gray { get; private set; }
        public static Texture Red { get; private set; }
        public static Texture Green { get; private set; }
        public static Texture Blue { get; private set; }

        public static void Init()
        {
            White = new Texture(new Vector3(1, 1, 1), nameof(White));
            Black = new Texture(new Vector3(0, 0, 0), nameof(Black));
            Gray = new Texture(new Vector3(0.5f, 0.5f, 0.5f), nameof(Gray));
            Red = new Texture(new Vector3(1, 0, 0), nameof(Red));
            Green = new Texture(new Vector3(0, 1, 0), nameof(Green));
            Blue = new Texture(new Vector3(0, 0, 1), nameof(Blue));
        }

        private static Dictionary<int, WeakReference<Texture>> References = new Dictionary<int, WeakReference<Texture>>();

        public static int ReferencedCount()
        {
            return References.Count;
        }

        internal static void AddRef(Texture texture)
        {
            lock (References)
                References.Set(texture.Handle, new WeakReference<Texture>(texture));
        }

        internal static void RemoveRef(Texture texture)
        {
            lock (References)
                References.Remove(texture.Handle);
        }

    }

}
