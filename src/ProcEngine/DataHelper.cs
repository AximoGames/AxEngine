using OpenTK;
using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace ProcEngine
{
    internal static class DataHelper
    {
        // Here we now have added the normals of the vertices
        // Remember to define the layouts to the VAO's
        public static readonly float[] Cube =
        {
             // Position          Normal
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f, // Bottom face
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f, // Top face
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f, // Left face
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f, // Right face
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f, // Front face
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f, // Back face
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };

        private const float dX = -0.5f;
        private const float dY = -0.5f;
        private const float dZ = 0.7f;
        public static readonly float[] CubeDebug =
        {
             // Position          Normal
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f, // Bottom face
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.6f, -0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f, // Top face
             0.6f, -0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.6f,  0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.6f,  0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
            -0.6f,  0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
            -0.6f, -0.6f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f, // Left face
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f, // Right face
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f, // Front face
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f, // Back face
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,

//--
            -0.1f+dX, -0.1f+dY, -0.3f+dZ,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f, // Bottom face
             0.1f+dX,  0.1f+dY, -0.3f+dZ,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.1f+dX, -0.1f+dY, -0.3f+dZ,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
            -0.1f+dX,  0.1f+dY, -0.3f+dZ,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
             0.1f+dX,  0.1f+dY, -0.3f+dZ,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.1f+dX, -0.1f+dY, -0.3f+dZ,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.1f+dX, -0.1f+dY,  0.3f+dZ,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f, // Top face
             0.1f+dX, -0.1f+dY,  0.3f+dZ,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.1f+dX,  0.1f+dY,  0.3f+dZ,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.1f+dX,  0.1f+dY,  0.3f+dZ,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
            -0.1f+dX,  0.1f+dY,  0.3f+dZ,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
            -0.1f+dX, -0.1f+dY,  0.3f+dZ,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,

            -0.1f+dX,  0.1f+dY,  0.3f+dZ, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f, // Left face
            -0.1f+dX,  0.1f+dY, -0.3f+dZ, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.1f+dX, -0.1f+dY, -0.3f+dZ, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.1f+dX, -0.1f+dY, -0.3f+dZ, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.1f+dX, -0.1f+dY,  0.3f+dZ, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.1f+dX,  0.1f+dY,  0.3f+dZ, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.1f+dX, -0.1f+dY, -0.3f+dZ,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f, // Right face
             0.1f+dX,  0.1f+dY, -0.3f+dZ,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.1f+dX,  0.1f+dY,  0.3f+dZ,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.1f+dX,  0.1f+dY,  0.3f+dZ,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.1f+dX, -0.1f+dY,  0.3f+dZ,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.1f+dX, -0.1f+dY, -0.3f+dZ,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,

            -0.1f+dX, -0.1f+dY, -0.3f+dZ,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f, // Front face
             0.1f+dX, -0.1f+dY, -0.3f+dZ,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.1f+dX, -0.1f+dY,  0.3f+dZ,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.1f+dX, -0.1f+dY,  0.3f+dZ,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.1f+dX, -0.1f+dY,  0.3f+dZ,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.1f+dX, -0.1f+dY, -0.3f+dZ,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

             0.1f+dX,  0.1f+dY, -0.3f+dZ,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f, // Back face
            -0.1f+dX,  0.1f+dY, -0.3f+dZ,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.1f+dX,  0.1f+dY,  0.3f+dZ,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.1f+dX,  0.1f+dY,  0.3f+dZ,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
             0.1f+dX,  0.1f+dY,  0.3f+dZ,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.1f+dX,  0.1f+dY, -0.3f+dZ,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };

        // Here we now have added the normals of the vertices
        // Remember to define the layouts to the VAO's
        public static readonly float[] Quad =
        {
            // vertex attributes for a quad that fills the entire screen in Normalized Device Coordinates.
            // positions   // texCoords
            -1.0f,  1.0f,  0.0f, 1.0f,
            -1.0f, -1.0f,  0.0f, 0.0f,
             1.0f, -1.0f,  1.0f, 0.0f,

            -1.0f,  1.0f,  0.0f, 1.0f,
             1.0f, -1.0f,  1.0f, 0.0f,
             1.0f,  1.0f,  1.0f, 1.0f
        };

        public static Bitmap GetDepthTexture(int width, int height, Action<IntPtr> getPixels)
        {
            Bitmap bitmap = new Bitmap(width, height);
            var ptr = Marshal.AllocHGlobal(width * height * 4);

            getPixels(ptr);

            var floats = new float[width * height];
            Marshal.Copy(ptr, floats, 0, width * height);
            Marshal.FreeHGlobal(ptr);

            var collection = floats.Where(v => v != 1);
            var min = collection.Min();
            var max = collection.Max();
            var span = max - min;

            var factor = 1.0f / span;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var value = floats[y * height + x];
                    if (value != 1.0f)
                    {
                        value -= min;
                        value *= factor;
                        //value = 1 - value;
                        //value *= 300;
                        //value = 1 - value;
                        var component = (int)(value * 255f);
                        component = Math.Max(0, Math.Min(component, 255));
                        var color = Color.FromArgb(component, component, component);
                        bitmap.SetPixel(x, y, color);
                    }
                }
            }

            bitmap.RotateFlip(System.Drawing.RotateFlipType.RotateNoneFlipY);

            return bitmap;
        }

        public static Matrix4 CoordinateSystemMatrix = new Matrix4(
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, -1, 0, 0),
            new Vector4(0, 0, 0, 1));

        public static Matrix3 CoordinateSystemMatrix3 = new Matrix3(
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, -1, 0));

        private const float LineGrayTone = 0.65f;
        public static readonly float[] Cross =
        {
             // Position          Color
             -1f, 0f, 0.0f,  LineGrayTone, LineGrayTone, LineGrayTone,  1.0f, // Line X
             1.0f,  0.0f, 0.0f,  LineGrayTone, LineGrayTone, LineGrayTone,  1.0f,

             0f, -1f, 0.0f,  LineGrayTone, LineGrayTone, LineGrayTone,  1.0f, // Line Y
             0.0f,  1.0f, 0.0f,  LineGrayTone, LineGrayTone, LineGrayTone,  1.0f,

             0f, 0f, -1.0f,  LineGrayTone, LineGrayTone, LineGrayTone,  1.0f, // Line Y
             0.0f,  0.0f, 1.0f,  LineGrayTone, LineGrayTone, LineGrayTone,  1.0f,
        };

    }

}
