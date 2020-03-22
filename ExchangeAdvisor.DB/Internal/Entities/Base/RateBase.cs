using System;
using System.ComponentModel.DataAnnotations.Schema;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.DB.Entities.Base
{
    public class RateBase : EntityBase
    {
        public DateTime Day { get; set; }

        public float Value { get; set; }

        [Column(TypeName = "nvarchar(4)")]
        public Currency BaseCurrency { get; set; }

        [Column(TypeName = "nvarchar(4)")]
        public Currency ComparingCurrency { get; set; }
    }
}
