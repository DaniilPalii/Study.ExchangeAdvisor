using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.Domain.Services.Implementation
{
    public class CacheableRateService : IRateService
    {
        public CacheableRateService(
            IWebRateHistoryFetcher webHistoryFetcher,
            IRateForecaster forecaster,
            IRateHistoryRepository historyRepository,
            IRateForecastRepository forecastRepository,
            IConfigurationReader configurationReader)
        {
            this.webHistoryFetcher = webHistoryFetcher;
            this.forecaster = forecaster;
            this.historyRepository = historyRepository;
            this.forecastRepository = forecastRepository;
            this.configurationReader = configurationReader;
        }

        public async Task<RateHistory> GetHistoryAsync(CurrencyPair currencyPair)
        {
            if (await historyRepository.ExistsAsync(currencyPair)
                && await historyRepository.GetLastDayAsync(currencyPair) >= DateTime.Today)
                return await historyRepository.GetAsync(currencyPair);

            var webRateHistory = await webHistoryFetcher.FetchAsync(HistoricalDateRange, currencyPair);

            return await historyRepository.AddOrUpdateAsync(webRateHistory);
        }

        public async Task<RateForecast> GetActualForecastAsync(CurrencyPair currencyPair)
        {
            if (await forecastRepository.ExistsAsync(currencyPair, DateTime.Today))
                return await GetSavedForecastAsync(currencyPair, DateTime.Today);

            var history = await historyRepository.GetAsync(currencyPair);
            var forecast = await forecaster.ForecastAsync(history, ForecastingDateRange);

            return await forecastRepository.AddOrUpdateAsync(forecast);
        }

        public Task<IEnumerable<RateForecastMetadata>> GetAllSavedForecastsMetadataAsync(CurrencyPair currencyPair)
        {
            return forecastRepository.GetAllForecastsMetadataAsync(currencyPair);
        }

        public Task<RateForecastMetadata> UpdateForecastMetadataAsync(RateForecastMetadata metadata)
        {
            return forecastRepository.UpdateMetadataAsync(metadata);
        }

        public Task<RateForecast> GetSavedForecastAsync(CurrencyPair currencyPair, DateTime creationDay)
        {
            return forecastRepository.GetAsync(currencyPair, creationDay);
        }

        private DateRange ForecastingDateRange => DateRange.FromToday().Until(configurationReader.ForecastingOffset);

        private DateRange HistoricalDateRange => DateRange.FromMinDate().UntilToday();

        private readonly IConfigurationReader configurationReader;
        private readonly IRateForecaster forecaster;
        private readonly IRateForecastRepository forecastRepository;
        private readonly IRateHistoryRepository historyRepository;
        private readonly IWebRateHistoryFetcher webHistoryFetcher;
    }
}