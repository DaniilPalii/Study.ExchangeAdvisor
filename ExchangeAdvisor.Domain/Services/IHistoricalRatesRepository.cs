using System.Collections.Generic;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IHistoricalRatesRepository
    {
        IEnumerable<Rate> Get(DateRange dateRange, CurrencyPair currencyPair);

        void Update(IEnumerable<Rate> entities);
    }
}
