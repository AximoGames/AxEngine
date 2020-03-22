// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using Aximo.Render;
using OpenTK;

namespace Aximo.Engine
{

    public class GameTexture
    {

        internal Texture InternalTexture;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2i Size => new Vector2i(Width, Height);

        private GameTexture(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public string SourcePath { get; private set; }
        public string Label { get; private set; }

        public static GameTexture GetFromFile(string sourcePath)
        {
            Console.WriteLine($"Loading: {sourcePath}");
            var imagePath = DirectoryHelper.GetAssetsPath(sourcePath);
            Bitmap bitmap;

            if (sourcePath.ToLower().EndsWith(".tga"))
                bitmap = TgaDecoder.FromFile(imagePath);
            else
                bitmap = new Bitmap(imagePath);

            var txt = new GameTexture(bitmap.Width, bitmap.Height);
            txt.SourcePath = sourcePath;
            txt.Label = Path.GetFileName(sourcePath);
            txt.AutoDisposeBitmap = true;
            txt.SetData(bitmap);
            return txt;
        }

        private bool AutoDisposeBitmap = false;

        public static GameTexture GetFromBitmap(Bitmap bitmap, string name = null, bool autoDisposeBitmap = false)
        {
            var txt = new GameTexture(bitmap.Width, bitmap.Height);
            txt.Label = name;
            txt.AutoDisposeBitmap = true;
            txt.SetData(bitmap);
            return txt;
        }

        private Bitmap Bitmap;

        private bool HasChanges;
        private bool BitmapChanged;

        public void SetData(Bitmap bitmap)
        {
            if (bitmap.Width != Width || bitmap.Height != Height)
                throw new InvalidOperationException();

            Bitmap = bitmap;
            BitmapChanged = true;
            HasChanges = true;
        }

        public void Sync()
        {
            if (!HasChanges)
                return;
            HasChanges = false;

            if (InternalTexture == null)
            {
                InternalTexture = new Texture(Bitmap);
                InternalTexture.ObjectLabel = Label;
            }
            else
            {
                if (BitmapChanged)
                {
                    BitmapChanged = false;
                    InternalTexture.SetData(Bitmap);
                    if (AutoDisposeBitmap)
                    {
                        Bitmap.Dispose();
                        Bitmap = null;
                    }
                }
            }
        }

    }

}
