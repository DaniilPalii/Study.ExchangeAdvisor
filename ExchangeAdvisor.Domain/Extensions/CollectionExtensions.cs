using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeAdvisor.Domain.Extensions
{
    public static class CollectionExtension
    {
        public static void Add<T>(this ICollection<T> collection, IEnumerable<T> values)
        {
            foreach (var value in values)
                collection.Add(value);
        }

        public static T GetRandomElement<T>(this IReadOnlyCollection<T> collection)
        {
            return collection.GetRandomElement(new Random());
        }

        public static T GetRandomElement<T>(this IReadOnlyCollection<T> collection, Random random)
        {
            var lastValueNumber = collection.Count - 1;
            var randomElementNumber = random.Next(maxValue: lastValueNumber);

            return collection.ElementAt(randomElementNumber);
        }
    }
}