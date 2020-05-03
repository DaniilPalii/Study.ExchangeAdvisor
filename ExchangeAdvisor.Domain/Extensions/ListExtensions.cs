using System.Collections.Generic;

namespace ExchangeAdvisor.Domain.Extensions
{
    public static class ListExtensions
    {
        public static void RemoveFromIndexToEnd<T>(this IList<T> list, int index)
        {
            while (list.Count > index)
                list.RemoveAt(list.Count - 1);
        }
    }
}