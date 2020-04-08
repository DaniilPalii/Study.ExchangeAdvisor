using System;
using System.Collections.Generic;

namespace ExchangeAdvisor.Domain.Helpers
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
    }
}