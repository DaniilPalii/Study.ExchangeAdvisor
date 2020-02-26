using System;

namespace ExchangeAdvisor.Domain.Values
{
    public struct CurrencyPair
    {
        public readonly Currency Base;

        public readonly Currency Comparing;

        public CurrencyPair(Currency @base, Currency comparing)
        {
            if (@base == comparing)
                throw new ArgumentException($"Currencies both are {@base} but should be different");

            Base = @base;
            Comparing = comparing;
        }

        public override bool Equals(object obj)
        {
            return obj is CurrencyPair currencyPair
                && Base == currencyPair.Base
                && Comparing == currencyPair.Comparing;
        }

        public override int GetHashCode() => (Base, Comparing).GetHashCode();

        public static bool operator ==(CurrencyPair left, CurrencyPair right) => left.Equals(right);

        public static bool operator !=(CurrencyPair left, CurrencyPair right) => !(left == right);
    }
}
