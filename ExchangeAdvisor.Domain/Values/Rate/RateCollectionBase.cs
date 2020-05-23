using System.Collections.Generic;
using System.Linq;

namespace ExchangeAdvisor.Domain.Values.Rate
{
    public abstract class RateCollectionBase
    {
        public virtual IReadOnlyCollection<Rate> Rates { get; }
        
        public virtual CurrencyPair CurrencyPair { get; }
        
        protected RateCollectionBase(IEnumerable<Rate> rates, CurrencyPair currencyPair) : this(rates)
        {
            CurrencyPair = currencyPair;
        }

        protected RateCollectionBase(IEnumerable<Rate> rates)
        {
            Rates = rates.OrderBy(r => r.Day).ToArray();
        }
    }
}