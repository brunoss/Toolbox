using System;

namespace ToolBox
{
    public static class StringUtils
    {
        public static string Reverse(this string value)
        {
            var aux = value.ToCharArray();
            aux.Reverse();
            return new string(aux);
        } 

        public static int Compare(this string value, string other, StringComparison comparison = StringComparison.Ordinal)
        {
            return string.Compare(value, other, comparison);
        }
    }
}
