using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.Domain.Services.Implementation
{
    public class RateService : IRateService
    {
        public RateService(
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

        public async Task RefreshSavedDataIfNeed(CurrencyPair currencyPair)
        {
            var missedHistoryRange = await GetMissedHistoryRangeOrNullAsync(currencyPair);

            if (missedHistoryRange == null)
                return;

            var webRateHistory = await webHistoryFetcher.FetchAsync(missedHistoryRange.Value, currencyPair);
            await historyRepository.AddOrUpdateAsync(webRateHistory);

            var updatedHistory = await historyRepository.GetAsync(currencyPair);
            var forecast = await forecaster.ForecastAsync(updatedHistory, ForecastingDateRange);
            await forecastRepository.AddAsync(forecast);
        }

        public async Task<RateHistory> GetHistoryAsync(CurrencyPair currencyPair, DateRange dateRange)
        {
            return await historyRepository.GetAsync(currencyPair, dateRange);
        }

        public Task<RateForecast> GetNewestForecastAsync(CurrencyPair currencyPair, DateRange dateRange)
        {
            return forecastRepository.GetNewestAsync(currencyPair, dateRange);
        }

        public Task<IEnumerable<RateForecastMetadata>> GetForecastsMetadataAsync(CurrencyPair currencyPair)
        {
            return forecastRepository.GetMetadatasAsync(currencyPair);
        }

        public async Task<RateForecastMetadata> UpdateForecastMetadataAsync(RateForecastMetadata metadata)
        {
            await forecastRepository.UpdateMetadataAsync(metadata);

            return await forecastRepository.GetMetadataAsync(metadata.CurrencyPair, metadata.CreationDay);
        }

        public Task<RateForecast> GetForecastAsync(CurrencyPair currencyPair, DateTime creationDay)
        {
            return forecastRepository.GetAsync(currencyPair, creationDay);
        }

        public Task<IEnumerable<RateForecast>> GetForecastsAsync(
            CurrencyPair currencyPair,
            IReadOnlyCollection<DateTime> creationDays)
        {
            return forecastRepository.GetAsync(currencyPair, creationDays);
        }

        private async Task<DateRange?> GetMissedHistoryRangeOrNullAsync(CurrencyPair currencyPair)
        {
            if (!await historyRepository.ExistsAsync(currencyPair))
                return DateRange.FromMinDate().UntilToday();

            var lastHistoricalDay = await historyRepository.GetLastDayAsync(currencyPair);
            if (lastHistoricalDay < DateTime.Today
                && await webHistoryFetcher.GetLatestRateDate(currencyPair) > lastHistoricalDay)
                return DateRange.From(lastHistoricalDay).UntilToday();

            return null;
        }

        private DateRange ForecastingDateRange => DateRange.FromToday().Until(configurationReader.ForecastingOffset);

        private readonly IConfigurationReader configurationReader;
        private readonly IRateForecaster forecaster;
        private readonly IRateForecastRepository forecastRepository;
        private readonly IRateHistoryRepository historyRepository;
        private readonly IWebRateHistoryFetcher webHistoryFetcher;
    }
}