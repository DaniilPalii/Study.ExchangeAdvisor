using ExchangeAdvisor.Domain.Values;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateService
    {
        Task<IEnumerable<Rate>> GetAsync(DateRange dateRange, CurrencyPair currencyPair);
    }
}