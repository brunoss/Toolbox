using System;

namespace ToolBox
{
    public static class StringUtils
    {
        public static string Reverse(this string value)
        {
            char[] charArray = new char[value.Length];
            for (int i = value.Length - 1; i >= 0; --i)
            {
                charArray[value.Length - i] = value[i];
            }
            return new string(charArray);
        }

        public static int Compare(this string value, string other, StringComparison comparison = StringComparison.Ordinal)
        {
            return string.Compare(value, other, comparison);
        }
    }
}
