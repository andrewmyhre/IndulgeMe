using System;
using System.Collections.Generic;
using System.Linq;

namespace BlessTheWeb.Core.Extensions
{
    public static class Extensions
    {
        static Random gen = new Random((int)DateTime.UtcNow.Ticks);

        public static IEnumerable<T> Random<T>(this IEnumerable<T> source, int count)
        {
            if (source.Count() == 0) return null;
            return source.Skip(gen.Next(0, source.Count() - 1) - 1).Take(count);

        }

        public static string Truncate(this string text, int maxLength)
        {
            return text.Substring(0, text.Length <= maxLength ? text.Length : maxLength);
        }
    }
}