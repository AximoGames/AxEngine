// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Text;
using Aximo.Generators.RandomNumbers;
using Cairo;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

// Reference: https://www.codeproject.com/Articles/838511/Procedural-seamless-noise-texture-generator

namespace Aximo.Generators.Voronoi
{
    public class VoronoiGeneratorOptions
    {
        public Vector4 Color1 { get; set; }
        public Vector4 Color2 { get; set; }
        public int Seed { get; set; }
        public Vector2i Size { get; set; }
        public int Points { get; set; }
        public int MinDelta { get; set; }

        public override string ToString()
        {
            return $"C1{Color1}C2{Color2}Seed{Seed}Size{Size}Points{Points}MinDelta{MinDelta}";
        }

    }

    public class VoronoiGenerator
    {

        public Image Voronoi3Image(Vector4 color1, Vector4 color2, int seed, Vector2i size, int points, int minDelta)
        {
            var rand = new Well512RandomNumberGenerator(seed);
            var pointArray = new Vector2i[points];
            for (var i = 0; i < points; i++)
                pointArray[i] = new Vector2i(rand.Next(size.X), rand.Next(size.Y));
            var values = Voronoi3(size.X, size.Y, pointArray, minDelta);

            var img = new Image<Rgba32>(size.X, size.Y);
            for (var x = 0; x < size.X; x++)
            {
                for (var y = 0; y < size.Y; y++)
                {
                    var value = values[x, y];
                    var result = Vector4.Lerp(color1, color2, value / 255f);
                    img[x, y] = new Rgba32(result.X, result.Y, result.Z, result.W);
                }
            }
            return img;
        }

        private float[,] Voronoi3(int width, int height, Vector2i[] points, int minDelta)
        {
            var values = new float[width, height];
            for (int ix = 0; ix < width; ix++)
            {
                for (int iy = 0; iy < height; iy++)
                {
                    float minDist = 999999999f;
                    float minDist2 = 999999999f;
                    for (int p = 0; p < points.GetLength(0); p++)
                    {
                        int pX = points[p].X;
                        int pY = points[p].Y;
                        double dist1X = Math.Abs(ix - pX);
                        double dist1Y = Math.Abs(iy - pY);
                        double dist2X = width - dist1X;
                        double dist2Y = height - dist1Y;
                        /*to grant seamless I take the min between distX and wid-distX
                         |                       |
                         |                       |     ----------- = Dist1X
                         |...i-----------X.......|     ..........  = Dist2X
                         |                       |
                         */
                        dist1X = Math.Min(dist1X, dist2X);
                        /*to grant seamless I take the min between distY and hei-distY*/
                        dist1Y = Math.Min(dist1Y, dist2Y);

                        float dist = (float)Math.Sqrt(Math.Pow(dist1X, 2) + Math.Pow(dist1Y, 2)); //euclidian metric

                        //float dist = (float)(Dist1X + Dist1Y);//Taxicab metric //http://en.wikipedia.org/wiki/Taxicab_geometry

                        //to make it ondulated
                        //1.
                        //dist = dist + (float)Math.Sin(Dist1X * 0.15) * (float)Math.Cos(Dist1Y * 0.15);
                        //2.
                        //dist = dist + (float)Math.Cos(Dist1Y * 0.15);
                        //dist = dist + (float)Math.Sin(Dist1Y * 0.15);

                        // strange effects:
                        // dist = dist % 100;
                        // dist = dist + dist % 100;

                        if (dist <= minDist)
                        {
                            minDist2 = minDist;
                            minDist = dist;
                        }
                        else
                        {
                            if (dist <= minDist2)
                                minDist2 = dist;
                        }
                    }

                    if (minDist2 - minDist < 1f)
                        values[ix, iy] = 0;
                    else
                        if (minDist2 - minDist < (float)minDelta)
                        values[ix, iy] = (minDist2 - minDist) / (float)minDelta * 255f;
                    else
                        values[ix, iy] = 255;
                }

            }

            return values;
        }
    }
}
