using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateFetcher
    {
        Task<IEnumerable<Rate>> FetchAsync(
            DateTime startDate,
            DateTime endDate,
            CurrencySymbol baseCurrency,
            CurrencySymbol comparingCurrency);

        Task<IEnumerable<Rate>> FetchAsync(
            DateTime startDate,
            DateTime endDate,
            CurrencySymbol baseCurrencySymbol);

        Task<IEnumerable<Rate>> FetchAsync(
            DateTime startDate,
            DateTime endDate);
    }
}