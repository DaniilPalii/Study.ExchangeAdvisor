using System;
using DomainRate = ExchangeAdvisor.Domain.Values.Rate.Rate;

namespace ExchangeAdvisor.DB.Entities
{
    public abstract class RateEntityBase : EntityBase
    {
        public DateTime Day { get; set; }

        public float Value { get; set; }

        protected RateEntityBase() { }

        protected RateEntityBase(DomainRate rate)
        {
            Day = rate.Day;
            Value = rate.Value;
        }

        public DomainRate ToDomain()
        {
            return new DomainRate(Day, Value);
        }
    }
}