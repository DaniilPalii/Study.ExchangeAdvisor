using System;

namespace ExchangeAdvisor.Domain.Values
{
    public class Rate
    {
        public DateTime Day { get; }

        public float Value { get; }

        public CurrencySymbol BaseCurrency { get; }

        public CurrencySymbol ComparingCurrency { get; }

        public Rate(DateTime day, float value, CurrencySymbol baseCurrency, CurrencySymbol comparingCurrency)
        {
            Day = day;
            Value = value;
            BaseCurrency = baseCurrency;
            ComparingCurrency = comparingCurrency;
        }
    }
}