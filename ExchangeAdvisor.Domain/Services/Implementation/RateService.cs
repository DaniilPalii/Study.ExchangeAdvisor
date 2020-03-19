using ExchangeAdvisor.Domain.Values;
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
            IHistoricalRatesRepository historicalRatesRepository)
        {
            this.rateWebFetcher = rateWebFetcher;
            this.rateForecaster = rateForecaster;
            this.historicalRatesRepository = historicalRatesRepository;
        }

        public async Task<IEnumerable<Rate>> GetAsync(DateRange dateRange, CurrencyPair currencyPair)
        {
            IEnumerable<Rate> rates;
            if (dateRange.End <= DateTime.Today)
            {
                rates = await GetFromFromWebWithCachingInRepository(dateRange, currencyPair);
            }
            else if (dateRange.Start > DateTime.Today)
            {
                rates = await rateForecaster.ForecastAsync(dateRange, currencyPair);
            }
            else
            {
                var gettingFromRepositoryTask = GetFromFromWebWithCachingInRepository(
                    DateRange.From(dateRange.Start).UntilToday(),
                    currencyPair);
                var forecastingTask = rateForecaster.ForecastAsync(
                    DateRange.From(DateTime.Today.AddDays(1)).Until(dateRange.End),
                    currencyPair);

                var historicalRates = await gettingFromRepositoryTask;
                var forecastedRates = await forecastingTask;
                rates = historicalRates.Concat(forecastedRates);
            }

            return rates.OrderBy(r => r.Day);
        }

        private async Task<ICollection<Rate>> GetFromFromWebWithCachingInRepository(DateRange dateRange, CurrencyPair currencyPair)
        {
            var ratesFromRepository = historicalRatesRepository.Get(dateRange, currencyPair)
                .ToArray();

            if (ratesFromRepository.Length > 0
                && ratesFromRepository.Max(r => r.Day) >= dateRange.End)
            {
                return ratesFromRepository;
            }

            await RefreshRepositoryRatesWithWebAsync(currencyPair);

            return historicalRatesRepository.Get(dateRange, currencyPair)
                .ToArray();
        }

        private async Task RefreshRepositoryRatesWithWebAsync(CurrencyPair currencyPair)
        {
            var dateRange = DateRange.FromMinDate().UntilToday();
            var ratesFromWeb = await rateWebFetcher.FetchAsync(dateRange, currencyPair);

            historicalRatesRepository.Update(ratesFromWeb);
        }

        private readonly IRateWebFetcher rateWebFetcher;
        private readonly IRateForecaster rateForecaster;
        private readonly IHistoricalRatesRepository historicalRatesRepository;
    }
}
