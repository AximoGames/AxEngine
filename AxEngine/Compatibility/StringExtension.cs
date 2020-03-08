namespace System
{
    public static class StringExtension
    {
        public static string[] Split(this string str, string seperator)
        {
            return str.Split(seperator.ToCharArray());
        }
    }
}