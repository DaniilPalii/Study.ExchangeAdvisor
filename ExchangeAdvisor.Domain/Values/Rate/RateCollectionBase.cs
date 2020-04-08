using System.Collections.Generic;

namespace ExchangeAdvisor.Domain.Values.Rate
{
    public abstract class RateCollectionBase
    {
        public virtual IEnumerable<Values.Rate.Rate> Rates { get; }
        
        public virtual CurrencyPair CurrencyPair { get; }
        
        protected RateCollectionBase(IEnumerable<Values.Rate.Rate> rates, CurrencyPair currencyPair) : this(rates)
        {
            CurrencyPair = currencyPair;
        }

        protected RateCollectionBase(IEnumerable<Values.Rate.Rate> rates)
        {
            Rates = rates;
        }
        
        protected RateCollectionBase() { }
    }
}