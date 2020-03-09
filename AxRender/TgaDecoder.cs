// This file is part of Aximo, a Game Engine written in C#. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.

// https://github.com/wertrain/tga-decoder-cs
// Licensed unter "The Unlicense"

using System;
using System.Drawing;
using System.IO;

namespace Aximo.Render
{
    public class TgaDecoder
    {
        protected class TgaData
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

            public TgaData(byte[] image) {
                this.idFieldLength = image[0];
                this.colorMapType = image[1];
                this.imageType = image[2];
                this.colorMapIndex = image[4] << 8 | image[3];
                this.colorMapLength = image[6] << 8 | image[5];
                this.colorMapDepth = image[7];
                this.imageOriginX = image[9] << 8 | image[8];
                this.imageOriginY = image[11] << 8 | image[10];
                this.imageWidth = image[13] << 8 | image[12];
                this.imageHeight = image[15] << 8 | image[14];
                this.bitPerPixel = image[16];
                this.descriptor = image[17];
                this.colorData = new byte[image.Length - TgaHeaderSize];
                Array.Copy(image, TgaHeaderSize, this.colorData, 0, this.colorData.Length);
                // Index color RLE or Full color RLE or Gray RLE
                if (this.imageType == 9 || this.imageType == 10 || this.imageType == 11)
                    this.colorData = this.DecodeRLE();
            }

            public int Width {
                get { return this.imageWidth; }
            }

            public int Height {
                get { return this.imageHeight; }
            }

            public int GetPixel(int x, int y) {
                if (colorMapType == 0) {
                    switch (this.imageType) {
                        // Index color
                        case 1:
                        case 9:
                            // not implemented
                            return 0;

                        // Full color
                        case 2:
                        case 10:
                            int elementCount = this.bitPerPixel / 8;
                            int dy = ((this.descriptor & 0x20) == 0 ? (this.imageHeight - 1 - y) : y) * this.imageWidth * elementCount;
                            int dx = ((this.descriptor & 0x10) == 0 ? x : (this.imageWidth - 1 - x)) * elementCount;
                            int index = dy + dx;

                            int b = this.colorData[index + 0] & 0xFF;
                            int g = this.colorData[index + 1] & 0xFF;
                            int r = this.colorData[index + 2] & 0xFF;

                            if (elementCount == 4) // this.bitPerPixel == 32
                            {
                                int a = this.colorData[index + 3] & 0xFF;
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
                else {
                    // not implemented
                    return 0;
                }
            }

            protected byte[] DecodeRLE() {
                int elementCount = this.bitPerPixel / 8;
                byte[] elements = new byte[elementCount];
                int decodeBufferLength = elementCount * this.imageWidth * this.imageHeight;
                byte[] decodeBuffer = new byte[decodeBufferLength];
                int decoded = 0;
                int offset = 0;
                while (decoded < decodeBufferLength) {
                    int packet = this.colorData[offset++] & 0xFF;
                    if ((packet & 0x80) != 0) {
                        for (int i = 0; i < elementCount; i++) {
                            elements[i] = this.colorData[offset++];
                        }
                        int count = (packet & 0x7F) + 1;
                        for (int i = 0; i < count; i++) {
                            for (int j = 0; j < elementCount; j++) {
                                decodeBuffer[decoded++] = elements[j];
                            }
                        }
                    }
                    else {
                        int count = (packet + 1) * elementCount;
                        for (int i = 0; i < count; i++) {
                            decodeBuffer[decoded++] = this.colorData[offset++];
                        }
                    }
                }
                return decodeBuffer;
            }
        }

        public static Bitmap FromFile(string path) {
            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    int length = (int)fs.Length;
                    byte[] buffer = new byte[length];
                    fs.Read(buffer, 0, length);
                    return Decode(buffer);
                }
            }
            catch (Exception) {
                return null;
            }
        }

        public static Bitmap FromBinary(byte[] image) {
            return Decode(image);
        }

        protected static Bitmap Decode(byte[] image) {
            TgaData tga = new TgaData(image);

            Bitmap bitmap = new Bitmap(tga.Width, tga.Height);
            for (int y = 0; y < tga.Height; ++y) {
                for (int x = 0; x < tga.Width; ++x) {
                    bitmap.SetPixel(x, y, Color.FromArgb(tga.GetPixel(x, y)));
                }
            }
            return bitmap;
        }

    }
}
