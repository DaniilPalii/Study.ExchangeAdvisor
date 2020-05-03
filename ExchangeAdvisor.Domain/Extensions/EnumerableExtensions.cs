﻿using System;
 using System.Collections.Generic;
 using System.Linq;

 namespace ExchangeAdvisor.Domain.Extensions
{
    public static class EnumerableExtension
    {
        public static bool HasAtLeast<T>(this IEnumerable<T> enumerable, int count)
        {
            using (var enumerator = enumerable.GetEnumerator())
            {
                if (count < 0)
                    throw new ArgumentException($"Expected enumerable count should be greater than 0, but was {count}");
                
                for (var i = 0; i < count; i++)
                {
                    var gotNextElement = enumerator.MoveNext();

                    if (!gotNextElement)
                        return false;
                }

                return true;
            }
        }

        public static IEnumerable<T> OrderRandomly<T>(this IEnumerable<T> enumerable)
        {
            var random = new Random();
            var remainingElements = enumerable.ToList();

            while (remainingElements.Count > 0)
            {
                var randomElement = remainingElements.GetRandomElement(random);
                remainingElements.Remove(randomElement);

                yield return randomElement;
            }
        }

        public static IEnumerable<T> RepeatEndlessly<T>(this IEnumerable<T> enumerable)
        {
            while (true)
            {
                foreach (var item in enumerable)
                {
                    yield return item;
                }
            }
        }

        public static IEnumerable<int> GetIndexes<T>(this IEnumerable<T> enumerable)
        {
            var index = 0;

            foreach (var item in enumerable)
            {
                yield return index++;
            }
        }
    }
}