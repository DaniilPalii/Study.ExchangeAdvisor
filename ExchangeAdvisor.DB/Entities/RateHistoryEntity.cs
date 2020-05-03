using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.DB.Entities
{
    [Table(name: "RateHistory")]
    public class RateHistoryEntity : EntityBase
    {
        public ICollection<HistoricalRateEntity> Rates { get; set; }
        
        [Column(TypeName = ColumnTypeName.Currency)]
        public Currency BaseCurrency { get; set; }

        [Column(TypeName = ColumnTypeName.Currency)]
        public Currency ComparingCurrency { get; set; }

        public RateHistoryEntity() { }

        public RateHistoryEntity(RateCollectionBase rateCollection)
        {
            Rates = rateCollection.Rates.Select(r => new HistoricalRateEntity(r)).ToArray();
            BaseCurrency = rateCollection.CurrencyPair.Base;
            ComparingCurrency = rateCollection.CurrencyPair.Comparing;
        }

        public RateHistory ToRateHistory()
        {
            return new RateHistory(
                Rates.Select(r => r.ToRate()),
                new CurrencyPair(BaseCurrency, ComparingCurrency));
        }
    }
}