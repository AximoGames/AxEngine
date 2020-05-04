// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using System;
using OpenToolkit.Mathematics;
using SixLabors.ImageSharp.Drawing.Processing;

namespace Aximo.Generators.AlchemyCircle
{
    public static class TextureDraw
    {
        public static void SetPixel(this Image image, int x, int y, Color color)
        {
            var img = (Image<Rgba32>)image;
            img[x, y] = color.ToPixel<Rgba32>();
        }

        public static void DrawLine(Image tex, int x0, int y0, int x1, int y1, Color col, int thickness)
        {
            tex.Mutate(ctx =>
            {
                var options = new ShapeGraphicsOptions();
                ctx.DrawLines(options, col, thickness, new PointF(x0, y0), new PointF(x1, y1));
            });
        }

        public static void DrawCircle(Image tex, int cx, int cy, int r, Color col, int thickness)
        {
            //int y = r;
            //float d = 1 / 4 - r;
            //float end = MathF.Ceil(r / MathF.Sqrt(2));

            //for (int x = 0; x <= end; x++)
            //{
            //    tex.SetPixel(cx + x, cy + y, col);
            //    tex.SetPixel(cx + x, cy - y, col);
            //    tex.SetPixel(cx - x, cy + y, col);
            //    tex.SetPixel(cx - x, cy - y, col);
            //    tex.SetPixel(cx + y, cy + x, col);
            //    tex.SetPixel(cx - y, cy + x, col);
            //    tex.SetPixel(cx + y, cy - x, col);
            //    tex.SetPixel(cx - y, cy - x, col);

            //    d += 2 * x + 1;
            //    if (d > 0)
            //    {
            //        d += 2 - 2 * y--;
            //    }
            //}
            for (int asd = 0; asd < thickness; asd++)
            {
                for (double i = 0.0; i < 360.0; i += 0.5)
                {
                    double angle = i * System.Math.PI / 180;
                    int x = (int)(cx + r * System.Math.Cos(angle));
                    int y = (int)(cy + r * System.Math.Sin(angle));
                    tex.SetPixel(x, y, col);
                }
                r++;
            }
        }
        public static void DrawFilledCircle(Image tex, int cx, int cy, int r, Color col, Color bgcol, int thickness)
        {
            for (int x = -r; x < r; x++)
            {
                int height = (int)MathF.Sqrt(r * r - x * x);

                for (int y = -height; y < height; y++)
                    tex.SetPixel(x + cx, y + cy, bgcol);
            }

            DrawCircle(tex, cx, cy, r, col, thickness);
        }



        private static Vector2[] GetPoints(int sides, float rot, float radius, float size)
        {
            Vector2[] values = new Vector2[sides];
            float angdiff = AxMath.Deg2Rad * (360 / (sides));
            rot = AxMath.Deg2Rad * (rot);
            for (int i = 0; i < sides; i++)
            {
                // trova i punti sulla circonferenza
                values[i].X = (size / 2) + radius * MathF.Cos(i * angdiff + rot); // X
                values[i].Y = (size / 2) + radius * MathF.Sin((i) * angdiff + rot); // Y
            }

            return values;
        }

        public static void DrawPolygon(Image tex, int n, int r, int rot, int size, Color col, int thickness)
        {
            if (n <= 0)
            {
                return;
            }

            Vector2[] p = GetPoints(n, rot, r, size);

            int i = 0;

            DrawLine(tex, (int)p[i].X, (int)p[i].Y, (int)p[n - 1].X, (int)p[n - 1].Y, col, thickness);

            for (i = 1; (i < n); i++)
            {
                DrawLine(tex, (int)p[i - 1].X, (int)p[i - 1].Y, (int)p[i].X, (int)p[i].Y, col, thickness);
            }
        }

        public static void DrawFilledPolygon(Image tex, int n, int r, int rot, int size, Color color, Color backgroundColor, int thickness)
        {

            Vector2[] p = GetPoints(n, rot, r, size);

            int i;
            int j;
            int index;
            int y;
            int miny, maxy, pmaxy;
            int x1, y1;
            int x2, y2;
            int ind1, ind2;
            int ints;
            int fill_color;
            if (n <= 0)
            {
                return;
            }

            miny = (int)p[0].Y;
            maxy = (int)p[0].Y;
            for (i = 1; (i < n); i++)
            {
                if (p[i].Y < miny)
                {
                    miny = (int)p[i].Y;
                }
                if (p[i].Y > maxy)
                {
                    maxy = (int)p[i].Y;
                }
            }
            /* necessary special case: horizontal line */
            if (n > 1 && miny == maxy)
            {
                x1 = x2 = (int)p[0].X;
                for (i = 1; (i < n); i++)
                {
                    if (p[i].X < x1)
                    {
                        x1 = (int)p[i].X;
                    }
                    else if (p[i].X > x2)
                    {
                        x2 = (int)p[i].X;
                    }
                }
                DrawLine(tex, x1, miny, x2, miny, backgroundColor, thickness);
                return;
            }
            pmaxy = maxy;

            // keep this inside the texture
            if (miny < 0)
            {
                miny = 0;
            }
            if (maxy > tex.Height - 1)
            {
                maxy = tex.Height - 1;
            }
            /* Fix in 1.3: count a vertex only once */
            for (y = miny; (y <= maxy); y++)
            {
                ints = 0;
                int[] polyInts = new int[n];
                for (i = 0; (i < n); i++)
                {
                    if (i == 0)
                    {
                        ind1 = n - 1;
                        ind2 = 0;
                    }
                    else
                    {
                        ind1 = i - 1;
                        ind2 = i;
                    }
                    y1 = (int)p[ind1].Y;
                    y2 = (int)p[ind2].Y;
                    if (y1 < y2)
                    {
                        x1 = (int)p[ind1].X;
                        x2 = (int)p[ind2].X;
                    }
                    else if (y1 > y2)
                    {
                        y2 = (int)p[ind1].Y;
                        y1 = (int)p[ind2].Y;
                        x2 = (int)p[ind1].X;
                        x1 = (int)p[ind2].X;
                    }
                    else
                    {
                        continue;
                    }
                    /* Do the following math as float intermediately, and round to ensure
                     * that Polygon and FilledPolygon for the same set of points have the
                     * same footprint. */

                    if ((y >= y1) && (y < y2))
                    {
                        polyInts[ints++] = (int)((float)((y - y1) * (x2 - x1)) /
                                                      (float)(y2 - y1) + 0.5 + x1);
                    }
                    else if ((y == pmaxy) && (y == y2))
                    {
                        polyInts[ints++] = x2;
                    }
                }


                // 2.0.26: polygons pretty much always have less than 100 points, and most of the time they have considerably less. For such trivial cases, insertion sort is a good choice. Also a good choice for future implementations that may wish to indirect through a table.

                for (i = 1; (i < ints); i++)
                {
                    index = polyInts[i];
                    j = i;
                    while ((j > 0) && (polyInts[j - 1] > index))
                    {
                        polyInts[j] = polyInts[j - 1];
                        j--;
                    }
                    polyInts[j] = index;
                }
                for (i = 0; (i < (ints - 1)); i += 2)
                {
                    // 2.0.29: back to gdImageLine to prevent segfaults when performing a pattern fill
                    DrawLine(tex, polyInts[i], y, polyInts[i + 1], y, backgroundColor, thickness);
                }
            }

            // draw border
            DrawPolygon(tex, n, r, rot, size, color, thickness);
        }
    }
}