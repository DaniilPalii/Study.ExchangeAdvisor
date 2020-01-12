using System;

namespace ExchangeAdvisor.Domain.Values
{
    public class Rate
    {
        public DateTime Day { get; }

        public double Value { get; }

        public CurrencySymbol BaseCurrency { get; }

        public CurrencySymbol ComparingCurrency { get; }

        public Rate(DateTime day, double value, CurrencySymbol baseCurrency, CurrencySymbol comparingCurrency)
        {
            Day = day;
            Value = value;
            BaseCurrency = baseCurrency;
            ComparingCurrency = comparingCurrency;
        }
    }
}