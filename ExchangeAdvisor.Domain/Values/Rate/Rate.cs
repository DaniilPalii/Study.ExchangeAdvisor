using System;

namespace ExchangeAdvisor.Domain.Values.Rate
{
    public class Rate
    {
        public DateTime Day { get; }

        public float Value { get; }

        public Rate(DateTime day, float value)
        {
            Day = day;
            Value = value;
        }

        public override bool Equals(object other)
        {
            return other is Rate rate
                && Day == rate.Day
                && Math.Abs(Value - rate.Value) < 0.005;
        }

        public override int GetHashCode() => (Day, Value).GetHashCode();
    }
}