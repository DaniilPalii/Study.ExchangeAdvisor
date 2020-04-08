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

        public bool Equals((Currency @base, Currency comparing) other) => Equals(other.@base, other.comparing);

        public bool Equals(Currency otherBase, Currency otherComparing)
        {
            return Base == otherBase
                && Comparing == otherComparing;
        }

        public override bool Equals(object other)
        {
            return other is CurrencyPair currencyPair
                && Base == currencyPair.Base
                && Comparing == currencyPair.Comparing;
        }

        public override int GetHashCode() => (Base, Comparing).GetHashCode();

        public static bool operator ==(CurrencyPair left, CurrencyPair right) => left.Equals(right);

        public static bool operator ==(CurrencyPair left, (Currency @base, Currency comparing) right) => left.Equals(right);

        public static bool operator ==((Currency @base, Currency comparing) left, CurrencyPair right) => right.Equals(left);

        public static bool operator !=(CurrencyPair left, CurrencyPair right) => !(left == right);

        public static bool operator !=(CurrencyPair left, (Currency @base, Currency comparing) right) => !(left == right);

        public static bool operator !=((Currency @base, Currency comparing) left, CurrencyPair right) => !(left == right);
    }
}
