namespace Aximo
{
    public static class StringExtensions
    {
        public static bool IsUnset(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsSet(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}
