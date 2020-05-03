using System.Collections.Generic;

namespace ExchangeAdvisor.Domain.Values.Rate
{
    public class RateHistory : RateCollectionBase
    {
        public RateHistory(IEnumerable<Rate> rates, CurrencyPair currencyPair)
            : base(rates, currencyPair)
        { }
    }
}