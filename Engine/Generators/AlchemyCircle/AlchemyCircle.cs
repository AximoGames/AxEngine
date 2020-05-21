// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Reference: https://github.com/CiaccoDavide/Alchemy-Circles-Generator
// Credits: CiaccoDavide

using System;
using System.Collections;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Aximo.Generators.AlchemyCircle
{
    public class AlchemyCircle
    {
        public Image Generate(int id, Color backgroundColor, Color color, int size, int thickness = 4)
        {
            CiaccoRandom randomer = new CiaccoRandom();
            randomer.SetSeed(id);

            var texture = new Image<Rgba32>(size, size);
            texture.Mutate(c => c.Clear(backgroundColor));

            // draw the hexagon:
            // hexagon's center coordinates and radius
            float hex_x = size / 2;
            float hex_y = size / 2;
            float radius = size / 2f * 3f / 4f;

            TextureDraw.DrawCircle(texture, size / 2, size / 2, (int)radius, color, thickness);

            int lati = randomer.GetRand(4, 8);

            TextureDraw.DrawPolygon(texture, lati, (int)radius, 0, size, color, thickness);
            int l;
            float ang;
            for (l = 0; l < lati; l++)
            {
                ang = AxMath.Deg2Rad * (360 / lati) * l;
                TextureDraw.DrawLine(texture, size / 2, size / 2, (int)((size / 2) + (radius * MathF.Cos(ang))), (int)((size / 2) + (radius * MathF.Sin(ang))), color, thickness);
            }
            int latis;
            if (lati % 2 == 0)
            {
                latis = randomer.GetRand(2, 6);

                while (latis % 2 != 0) latis = randomer.GetRand(3, 6);

                TextureDraw.DrawFilledPolygon(texture, latis, (int)radius, 180, size, color, backgroundColor, thickness);

                for (l = 0; l < latis; l++)
                {
                    ang = AxMath.Deg2Rad * (360 / latis) * l;
                    TextureDraw.DrawLine(texture, size / 2, size / 2, (int)((size / 2) + (radius * MathF.Cos(ang))), (int)((size / 2) + (radius * MathF.Sin(ang))), color, thickness);
                }
            }
            else
            {
                latis = randomer.GetRand(2, 6);
                while (latis % 2 == 0) latis = randomer.GetRand(3, 6);

                TextureDraw.DrawFilledPolygon(texture, latis, (int)radius, 180, size, color, backgroundColor, thickness);
            }

            if (randomer.GetRand(0, 1) % 2 == 0)
            {
                int ronad = randomer.GetRand(0, 4);

                if (ronad % 2 == 1)
                {
                    for (l = 0; l < lati + 4; l++)
                    {
                        ang = AxMath.Deg2Rad * (360 / (lati + 4)) * l;
                        TextureDraw.DrawLine(texture, size / 2, size / 2, (int)((size / 2) + (((radius / 8 * 5) + 2) * MathF.Cos(ang))), (int)((size / 2) + (((radius / 8 * 5) + 2) * MathF.Sin(ang))), color, thickness);
                    }

                    TextureDraw.DrawFilledPolygon(texture, lati + 4, (int)(radius / 2), 0, size, color, backgroundColor, thickness);
                }
                else if (ronad % 2 == 0)
                {
                    for (l = 0; l < lati - 2; l++)
                    {
                        ang = AxMath.Deg2Rad * (360 / (lati - 2)) * l;
                        TextureDraw.DrawLine(texture, size / 2, size / 2, (int)((size / 2) + (((radius / 8 * 5) + 2) * MathF.Cos(ang))), (int)((size / 2) + (((radius / 8 * 5) + 2) * MathF.Sin(ang))), color, thickness);
                    }

                    TextureDraw.DrawFilledPolygon(texture, lati - 2, (int)(radius / 4), 0, size, color, backgroundColor, thickness);
                }
            }

            if (randomer.GetRand(0, 4) % 2 == 0)
            {
                TextureDraw.DrawCircle(texture, size / 2, size / 2, (int)(radius / 16f * 11f), color, thickness);

                if (lati % 2 == 0)
                {
                    latis = randomer.GetRand(2, 8);

                    while (latis % 2 != 0) latis = randomer.GetRand(3, 8);

                    TextureDraw.DrawPolygon(texture, latis, (int)(radius / 3 * 2), 180, size, color, thickness);
                }
                else
                {
                    latis = randomer.GetRand(2, 8);

                    while (latis % 2 == 0) latis = randomer.GetRand(3, 8);

                    TextureDraw.DrawPolygon(texture, latis, (int)(radius / 3 * 2), 180, size, color, thickness);
                }
            }

            int caso = randomer.GetRand(0, 3);
            float angdiff, posax, posay;
            if (caso == 0)
            {
                for (int i = 0; i < latis; i++)
                {
                    angdiff = AxMath.Deg2Rad * (360 / latis);
                    posax = radius / 18 * 11 * MathF.Cos(i * angdiff);
                    posay = radius / 18 * 11 * MathF.Sin(i * angdiff);
                    TextureDraw.DrawFilledCircle(texture, (int)((size / 2) + posax), (int)((size / 2) + posay), (int)(radius / 44 * 6), color, backgroundColor, thickness);
                }
            }
            else if (caso == 1)
            {
                for (int i = 0; i < latis; i++)
                {
                    angdiff = AxMath.Deg2Rad * (360 / latis);
                    posax = radius * MathF.Cos(i * angdiff);
                    posay = radius * MathF.Sin(i * angdiff);
                    TextureDraw.DrawFilledCircle(texture, (int)((size / 2) + posax), (int)((size / 2) + posay), (int)(radius / 44 * 6), color, backgroundColor, thickness);
                }
            }
            else if (caso == 2)
            {
                TextureDraw.DrawCircle(texture, size / 2, size / 2, (int)(radius / 18 * 6), color, thickness);
                TextureDraw.DrawFilledCircle(texture, size / 2, size / 2, (int)(radius / 22 * 6), color, backgroundColor, thickness);
            }
            else if (caso == 3)
            {
                for (int i = 0; i < latis; i++)
                {
                    ang = AxMath.Deg2Rad * (360 / latis) * i;
                    TextureDraw.DrawLine(texture, (int)((size / 2) + (radius / 3 * 2 * MathF.Cos(ang))), (int)((size / 2) + (radius / 3 * 2 * MathF.Sin(ang))), (int)((size / 2) + (radius * MathF.Cos(ang))), (int)((size / 2) + (radius * MathF.Sin(ang))), color, thickness);
                }
                if (latis == lati)
                {
                }
                else
                {
                    TextureDraw.DrawFilledCircle(texture, size / 2, size / 2, (int)(radius / 3 * 2), color, backgroundColor, thickness);
                    lati = randomer.GetRand(3, 6);
                    TextureDraw.DrawPolygon(texture, lati, (int)(radius / 4 * 5), 0, size, color, thickness);
                    TextureDraw.DrawPolygon(texture, lati, (int)(radius / 3 * 2), 180, size, color, thickness);
                }
            }

            return texture;
        }
    }
}
