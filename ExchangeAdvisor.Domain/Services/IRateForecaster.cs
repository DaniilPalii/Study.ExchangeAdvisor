using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateForecaster
    {
        Task<IEnumerable<Rate>> ForecastAsync(ICollection<Rate> historicalRates, DateRange dateRange);
    }
}
