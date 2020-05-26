// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// https://github.com/wertrain/tga-decoder-cs
// Licensed unter "The Unlicense"

using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Aximo
{
    internal static class TgaDecoder
    {
        private class TgaData
        {
            private const int TgaHeaderSize = 18;
            private int idFieldLength;
            private int colorMapType;
            private int imageType;
            private int colorMapIndex;
            private int colorMapLength;
            private int colorMapDepth;
            private int imageOriginX;
            private int imageOriginY;
            private int imageWidth;
            private int imageHeight;
            private int bitPerPixel;
            private int descriptor;
            private byte[] colorData;

            public TgaData(byte[] image)
            {
                idFieldLength = image[0];
                colorMapType = image[1];
                imageType = image[2];
                colorMapIndex = image[4] << 8 | image[3];
                colorMapLength = image[6] << 8 | image[5];
                colorMapDepth = image[7];
                imageOriginX = image[9] << 8 | image[8];
                imageOriginY = image[11] << 8 | image[10];
                imageWidth = image[13] << 8 | image[12];
                imageHeight = image[15] << 8 | image[14];
                bitPerPixel = image[16];
                descriptor = image[17];
                colorData = new byte[image.Length - TgaHeaderSize];
                Array.Copy(image, TgaHeaderSize, colorData, 0, colorData.Length);
                // Index color RLE or Full color RLE or Gray RLE
                if (imageType == 9 || imageType == 10 || imageType == 11)
                    colorData = DecodeRLE();
            }

            public int Width
            {
                get { return imageWidth; }
            }

            public int Height
            {
                get { return imageHeight; }
            }

            public int GetPixel(int x, int y)
            {
                if (colorMapType == 0)
                {
                    switch (imageType)
                    {
                        // Index color
                        case 1:
                        case 9:
                            // not implemented
                            return 0;

                        // Full color
                        case 2:
                        case 10:
                            int elementCount = bitPerPixel / 8;
                            int dy = ((descriptor & 0x20) == 0 ? (imageHeight - 1 - y) : y) * imageWidth * elementCount;
                            int dx = ((descriptor & 0x10) == 0 ? x : (imageWidth - 1 - x)) * elementCount;
                            int index = dy + dx;

                            int b = colorData[index + 0] & 0xFF;
                            int g = colorData[index + 1] & 0xFF;
                            int r = colorData[index + 2] & 0xFF;

                            if (elementCount == 4) // this.bitPerPixel == 32
                            {
                                int a = colorData[index + 3] & 0xFF;
                                return (a << 24) | (r << 16) | (g << 8) | b;
                            }
                            else if (elementCount == 3) // this.bitPerPixel == 24
                            {
                                return (r << 16) | (g << 8) | b;
                            }
                            break;

                        // Gray
                        case 3:
                        case 11:
                            // not implemented
                            return 0;
                    }
                    return 0;
                }
                else
                {
                    // not implemented
                    return 0;
                }
            }

            protected byte[] DecodeRLE()
            {
                int elementCount = bitPerPixel / 8;
                byte[] elements = new byte[elementCount];
                int decodeBufferLength = elementCount * imageWidth * imageHeight;
                byte[] decodeBuffer = new byte[decodeBufferLength];
                int decoded = 0;
                int offset = 0;
                while (decoded < decodeBufferLength)
                {
                    int packet = colorData[offset++] & 0xFF;
                    if ((packet & 0x80) != 0)
                    {
                        for (int i = 0; i < elementCount; i++)
                        {
                            elements[i] = colorData[offset++];
                        }
                        int count = (packet & 0x7F) + 1;
                        for (int i = 0; i < count; i++)
                        {
                            for (int j = 0; j < elementCount; j++)
                            {
                                decodeBuffer[decoded++] = elements[j];
                            }
                        }
                    }
                    else
                    {
                        int count = (packet + 1) * elementCount;
                        for (int i = 0; i < count; i++)
                        {
                            decodeBuffer[decoded++] = colorData[offset++];
                        }
                    }
                }
                return decodeBuffer;
            }
        }

        public static Image FromFile(string path)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    int length = (int)fs.Length;
                    byte[] buffer = new byte[length];
                    fs.Read(buffer, 0, length);
                    return Decode(buffer);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Image FromBinary(byte[] image)
        {
            return Decode(image);
        }

        private static Image Decode(byte[] image)
        {
            TgaData tga = new TgaData(image);

            var bitmap = new Image<Argb32>(tga.Width, tga.Height);
            for (int y = 0; y < tga.Height; ++y)
            {
                for (int x = 0; x < tga.Width; ++x)
                {
                    unchecked
                    {
                        bitmap[x, y] = new Argb32((uint)tga.GetPixel(x, y));
                    }
                }
            }
            return bitmap;
        }
    }
}
