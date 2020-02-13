using ExchangeAdvisor.DB.Values;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeAdvisor.DB.Entities.Base
{
    public class RateBase : EntityBase
    {
        [Key]
        public DateTime Day { get; set; }

        public float Value { get; set; }

        public CurrencySymbol BaseCurrency { get; set; }

        public CurrencySymbol ComparingCurrency { get; set; }
    }
}
