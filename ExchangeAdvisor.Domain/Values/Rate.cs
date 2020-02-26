using System;

namespace ExchangeAdvisor.Domain.Values
{
    public class Rate
    {
        public DateTime Day { get; }

        public float Value { get; }

        public CurrencyPair CurrencyPair { get; }

        public Rate(DateTime day, float value, CurrencyPair currencyPair)
        {
            Day = day;
            Value = value;
            CurrencyPair = currencyPair;
        }

        public override bool Equals(object obj)
        {
            return obj is Rate rate
                && Day == rate.Day
                && Value == rate.Value
                && CurrencyPair == rate.CurrencyPair;
        }

        public override int GetHashCode() => (Day, Value, CurrencyPair).GetHashCode();
    }
}