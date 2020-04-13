using System.Collections.Generic;
using System.Linq;

namespace ExchangeAdvisor.Domain.Values.Rate
{
    public abstract class RateCollectionBase
    {
        public virtual IEnumerable<Rate> Rates { get; private set; }
        
        public virtual CurrencyPair CurrencyPair { get; }
        
        protected RateCollectionBase(IEnumerable<Rate> rates, CurrencyPair currencyPair) : this(rates)
        {
            CurrencyPair = currencyPair;
        }

        protected RateCollectionBase(IEnumerable<Rate> rates)
        {
            Rates = rates;
        }
        
        protected RateCollectionBase() { }

        public void CalculateRates()
        {
            Rates = Rates.ToArray();
        }
    }
}