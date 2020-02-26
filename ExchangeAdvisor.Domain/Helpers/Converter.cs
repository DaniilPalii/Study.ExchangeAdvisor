using System;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Helpers
{
    public static class Converter
    {
        public static Currency ToCurrency(string currencySymbol)
        {
            return ToEnum<Currency>(currencySymbol);
        }

        public static T ToEnum<T>(string enumValueName) where T : struct
        {
            return (T)Enum.Parse(typeof(T), enumValueName);
        }
    }
}
