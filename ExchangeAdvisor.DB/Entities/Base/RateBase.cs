using System;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.DB.Entities.Base
{
    internal class RateBase : EntityBase
    {
        public DateTime Day { get; set; }

        public float Value { get; set; }

        public Currency BaseCurrency { get; set; }

        public Currency ComparingCurrency { get; set; }
    }
}
