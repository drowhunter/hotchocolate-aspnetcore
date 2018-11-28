using System;

namespace HotChocolate.AspNetCore
{
    internal static class StringExtensions
    {
        public static bool EqualsOrdinal(this string s, string other)
        {
            return string.Equals(s, other, StringComparison.Ordinal);
        }
    }
}
