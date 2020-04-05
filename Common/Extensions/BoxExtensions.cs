using OpenToolkit.Mathematics;
using SixLabors.ImageSharp;

namespace Aximo
{

    public static class RectangleExtensions
    {
        public static Box2 ToBox2(this RectangleF rect)
        {
            return new Box2(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
    }

    public static class BoxHelper
    {
        public static Box2 FromSize(Vector2 location, Vector2 size)
        {
            return new Box2(location, location + size);
        }

    }

    public static class BoxExtensions
    {
        public static RectangleF ToRectangleF(this Box2 value)
        {
            return new RectangleF(value.Min.X, value.Min.Y, value.Size.X, value.Size.Y);
        }

    }

}
