using ExchangeAdvisor.Domain.Values;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateService
    {
        Task<IEnumerable<Rate>> FetchHistoricalRatesAsync(
            CurrencySymbol baseCurrency,
            CurrencySymbol comparingCurrency);
    }
}