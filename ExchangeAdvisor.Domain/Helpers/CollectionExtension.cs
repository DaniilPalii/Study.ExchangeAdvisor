using System.Collections.Generic;

namespace ExchangeAdvisor.Domain.Helpers
{
    public static class CollectionExtension
    {
        public static void Add<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            foreach (var value in values)
                collection.Add(value);
        }
    }
}