using ExchangeAdvisor.DB.Entities;
using ExchangeAdvisor.DB.Repositories;
using ExchangeAdvisor.Domain.Values;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeAdvisor.Domain.Services.Implementation
{
    public class RateService : IRateService
    {
        public RateService(
            IRateWebFetcher rateWebFetcher,
            IRateForecaster rateForecaster,
            IRepository<HistoricalRate> historicalRatesRepository)
        {
            this.rateWebFetcher = rateWebFetcher;
            this.rateForecaster = rateForecaster;
            this.historicalRatesRepository = historicalRatesRepository;
        }

        public async Task<IEnumerable<Rate>> FetchHistoricalRatesAsync(
            CurrencySymbol baseCurrency,
            CurrencySymbol comparingCurrency)
        {
            var ratesFromRepository = historicalRatesRepository.GetAll();

            if (ratesFromRepository.Count == 0)
            {
                ratesFromRepository = await RefreshRepositoryRatesWithWeb(
                    baseCurrency,
                    comparingCurrency,
                    startDate: DateTime.MinValue);
            }
            else if (ratesFromRepository.Max(r => r.Day) < DateTime.Today)
            {
                ratesFromRepository = await RefreshRepositoryRatesWithWeb(
                    baseCurrency,
                    comparingCurrency,
                    startDate: DateTime.Today);
            }

            return ratesFromRepository.Adapt<IEnumerable<Rate>>()
                .OrderBy(r => r.Day)
                .ThenBy(r => r.BaseCurrency)
                .ThenBy(r => r.ComparingCurrency);
        }

        private async Task<ICollection<HistoricalRate>> RefreshRepositoryRatesWithWeb(
            CurrencySymbol baseCurrency,
            CurrencySymbol comparingCurrency,
            DateTime startDate)
        {
            var ratesFromWeb = await rateWebFetcher.FetchAsync(
                startDate: startDate,
                endDate: DateTime.Today,
                baseCurrency,
                comparingCurrency);

            historicalRatesRepository.Update(ratesFromWeb.Adapt<IEnumerable<HistoricalRate>>());

            return historicalRatesRepository.GetAll();
        }

        private readonly IRateWebFetcher rateWebFetcher;
        private readonly IRateForecaster rateForecaster;
        private readonly IRepository<HistoricalRate> historicalRatesRepository;
    }
}
