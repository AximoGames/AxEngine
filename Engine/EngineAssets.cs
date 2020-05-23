// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Aximo.Engine
{
    internal static class EngineAssets
    {
        public static void Init()
        {
            DirectoryHelper.AddFileGenerator(EmbeddedRessource);
            DirectoryHelper.AddFileGenerator("Textures/Engine/UVTest.png", CreateImage);
            DirectoryHelper.AddFileGenerator("Textures/AlchemyCircle/.png", AlchemyCircle);
        }

        private static bool AlchemyCircle(string subPath, string cachePath, object options)
        {
            var opt = (Generators.AlchemyCircle.AlchemyCircleOptions)options;
            new Generators.AlchemyCircle.AlchemyCircleGenerator().Generate(opt.Seed, opt.BackgroundColor, opt.Color, opt.Size, opt.Thickness).Save(cachePath);
            return true;
        }

        private static bool EmbeddedRessource(string subPath, string cachePath, object options)
        {
            var asm = typeof(Render.Camera).Assembly;
            var resName = asm.GetName().Name + "." + subPath.Replace("/", ".").Replace("\\", ".");
            var names = asm.GetManifestResourceNames();
            if (!names.Contains(resName))
                return false;

            File.WriteAllBytes(cachePath, asm.GetManifestResourceStream(resName).ReadToEnd());

            return true;
        }

        private static bool CreateImage(string subPath, string cachePath, object options)
        {
            var img = new Image<Rgb24>(512, 512);
            img.Mutate(ctx => ctx.Clear(Color.White));
            var blackBrush = Brushes.Solid(Color.Black);
            var whiteBrush = Brushes.Solid(Color.White);
            var charsX = "12345678";
            var charsY = "ABCDEFGH";
            var font = new Font(SystemFonts.Families.First(), 56f, FontStyle.Regular);

            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    var box = new Rectangle(x * 64, y * 64, 64, 64);
                    var label = (string)charsY[y].ToString() + charsX[x].ToString();

                    var evenX = x % 2 == 0;
                    var evenY = y % 2 == 0;
                    var black = evenX;
                    if (evenY)
                        black = !black;

                    if (black)
                        img.Mutate(ctx => ctx.Fill(blackBrush, box));

                    var fontColor = black ? Color.White : Color.Black;

                    var gfxOptions = new TextGraphicsOptions
                    {
                        TextOptions = new TextOptions
                        {
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                        },
                    };
                    img.Mutate(ctx => ctx.DrawText(gfxOptions, label, font, fontColor, new PointF(box.X + 32, box.Y + 32)));
                }
            }
            img.Save(cachePath);
            return true;
        }
    }
}
