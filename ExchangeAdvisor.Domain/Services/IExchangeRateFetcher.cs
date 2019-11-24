using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IExchangeRateFetcher
    {
        Task<IEnumerable<RateOnDay>> FetchRateHistoryAsync(
            DateTime startDate,
            DateTime endDate,
            CurrencySymbol baseCurrencySymbol,
            CurrencySymbol comparingCurrencySymbol);
    }
}