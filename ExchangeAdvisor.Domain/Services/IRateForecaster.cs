using System.Collections.Generic;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateForecaster
    {
        IEnumerable<Rate> Forecast(DateRange dateRange, CurrencyPair currencyPair);
    }
}
