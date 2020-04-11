using OpenToolkit.Mathematics;

namespace Aximo
{
    public static class VectorExtensions
    {
        public static Vector2i InvertY(this Vector2i value)
        {
            return new Vector2i(value.X, -value.Y);
        }

        public static Vector2 InvertY(this Vector2 value)
        {
            return new Vector2(value.X, -value.Y);
        }

    }

}
