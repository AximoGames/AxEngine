﻿// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using Aximo.Render.OpenGL;
using OpenToolkit.Mathematics;
using Image = SixLabors.ImageSharp.Image;

namespace Aximo.Engine
{
    public class Texture : SceneObject
    {
        private static Serilog.ILogger Log = Aximo.Log.ForContext<Texture>();

        internal RendererTexture RendererTexture;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Vector2i Size => new Vector2i(Width, Height);

        private Texture(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public string SourcePath { get; private set; }
        public string Label { get; private set; }

        internal override void DoDeallocation()
        {
            if (!HasDeallocation)
                return;

            if (RendererTexture == null)
                return;

            Log.Verbose("Set InternalTexture.Orphaned");

            RendererTexture.Orphaned = true;
            RendererTexture = null;

            base.DoDeallocation();
        }

        public static Texture CreateFromFile(string sourcePath)
        {
            Log.Info("Loading: {SourcePath}", sourcePath);
            var imagePath = AssetManager.GetAssetsPath(sourcePath);
            Image bitmap = Image.Load(imagePath);

            var txt = new Texture(bitmap.Width, bitmap.Height)
            {
                SourcePath = sourcePath,
                Label = Path.GetFileName(sourcePath),
                AutoDisposeBitmap = true,
            };
            txt.SetData(bitmap);
            return txt;
        }

        public static Texture GetFromFile(string sourcePath)
        {
            return TextureManager.GetFromFile(sourcePath);
        }

        private bool AutoDisposeBitmap = false;

        public static Texture GetFromBitmap(Image bitmap, string name = null, bool autoDisposeBitmap = false)
        {
            var txt = new Texture(bitmap.Width, bitmap.Height)
            {
                Label = name,
                AutoDisposeBitmap = autoDisposeBitmap,
            };
            txt.SetData(bitmap);
            return txt;
        }

        private Image Bitmap;

        private bool HasChanges;
        private bool BitmapChanged;

        public void SetData(Image bitmap)
        {
            // if (bitmap.Width != Width || bitmap.Height != Height)
            //     throw new InvalidOperationException();

            Bitmap = bitmap;
            BitmapChanged = true;
            HasChanges = true;
        }

        public void Sync()
        {
            if (!HasChanges)
                return;
            HasChanges = false;

            if (RendererTexture == null)
            {
                RendererTexture = new RendererTexture(Bitmap, Label);
            }
            else
            {
                if (BitmapChanged)
                {
                    BitmapChanged = false;
                    RendererTexture.SetData(Bitmap);
                    if (AutoDisposeBitmap)
                    {
                        Log.Verbose("Disposing Bitmap for {ObjectLabel}", RendererTexture.ObjectLabel);
                        Bitmap.Dispose();
                        Bitmap = null;
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RendererTexture?.Dispose();
            }
            RendererTexture = null;
            base.Dispose(disposing);
        }
    }
}
