using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.DB.Entities
{
    public abstract class RateCollectionEntityBase : EntityBase
    {
        [Column(TypeName = CurrencyColumnTypeName)]
        public Currency BaseCurrency { get; set; }

        [Column(TypeName = CurrencyColumnTypeName)]
        public Currency ComparingCurrency { get; set; }

        public ICollection<RateEntityBase> Rates { get; set; }

        protected const string CurrencyColumnTypeName = "nvarchar(4)";
    }
}
