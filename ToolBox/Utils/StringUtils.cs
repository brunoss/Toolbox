using System;

namespace ToolBox
{
    public static class StringUtils
    {
        public static string Reverse(this string value)
        {
            var reverse = value.ToCharArray();
            for (int i = 0; i < value.Length/2; ++i)
            {
                var aux = reverse[i];
                reverse[i] = reverse[reverse.Length - i - 1];
                reverse[reverse.Length - i - 1] = aux;
            }
            return new string(reverse);
        } 

        public static int Compare(this string value, string other, StringComparison comparison = StringComparison.Ordinal)
        {
            return string.Compare(value, other, comparison);
        }
    }
}
