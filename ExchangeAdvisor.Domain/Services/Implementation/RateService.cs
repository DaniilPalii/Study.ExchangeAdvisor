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
            if (await historyRepository.ExistsAsync(currencyPair))
            {
                var lastHistoricalDayAsync = await historyRepository.GetLastDayAsync(currencyPair);

                if (lastHistoricalDayAsync < DateTime.Today)
                {
                    var webRateHistory = await webHistoryFetcher.FetchAsync(HistoricalDateRange, currencyPair);
                    webRateHistory.CalculateRates();

                    if (webRateHistory.Rates.Max(r => r.Day) > lastHistoricalDayAsync) // TODO: fetch from web only latest day
                    {
                        await historyRepository.AddOrUpdateAsync(webRateHistory);
                        
                        var updatedHistory = await GetHistoryAsync(currencyPair);
                        var forecast = await forecaster.ForecastAsync(updatedHistory, ForecastingDateRange);
                        await forecastRepository.AddAsync(forecast);
                    }
                }
            }
            else
            {
                var webRateHistory = await webHistoryFetcher.FetchAsync(HistoricalDateRange, currencyPair);
                await historyRepository.AddOrUpdateAsync(webRateHistory);
                
                var updatedHistory = await GetHistoryAsync(currencyPair);
                var forecast = await forecaster.ForecastAsync(updatedHistory, ForecastingDateRange);
                await forecastRepository.AddAsync(forecast);
            }
        }

        public async Task<RateHistory> GetHistoryAsync(CurrencyPair currencyPair)
        {
            return await historyRepository.GetAsync(currencyPair);
        }

        public Task<RateForecast> GetNewestForecastAsync(CurrencyPair currencyPair)
        {
            return forecastRepository.GetNewestAsync(currencyPair);
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

        private DateRange ForecastingDateRange => DateRange.FromToday().Until(configurationReader.ForecastingOffset);

        private DateRange HistoricalDateRange => DateRange.FromMinDate().UntilToday();

        private readonly IConfigurationReader configurationReader;
        private readonly IRateForecaster forecaster;
        private readonly IRateForecastRepository forecastRepository;
        private readonly IRateHistoryRepository historyRepository;
        private readonly IWebRateHistoryFetcher webHistoryFetcher;
    }
}