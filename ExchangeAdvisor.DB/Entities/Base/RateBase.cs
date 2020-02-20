using ExchangeAdvisor.DB.Values;
using System;

namespace ExchangeAdvisor.DB.Entities.Base
{
    public class RateBase : EntityBase
    {
        public DateTime Day { get; set; }

        public float Value { get; set; }

        public CurrencySymbol BaseCurrency { get; set; }

        public CurrencySymbol ComparingCurrency { get; set; }
    }
}
