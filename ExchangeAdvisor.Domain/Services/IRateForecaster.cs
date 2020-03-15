using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateForecaster
    {
        Task<IEnumerable<Rate>> ForecastAsync(DateRange dateRange, CurrencyPair currencyPair);
    }
}
