using ExchangeAdvisor.Domain.Values;
using System;

namespace ExchangeAdvisor.Domain
{
    public static class Converter
    {
        public static CurrencySymbol ToCurrencySymbol(string currencySymbolName)
        {
            return ToEnum<CurrencySymbol>(currencySymbolName);
        }

        public static T ToEnum<T>(string enumValueName) where T : struct
        {
            return (T)Enum.Parse(typeof(T), enumValueName);
        }
    }
}
