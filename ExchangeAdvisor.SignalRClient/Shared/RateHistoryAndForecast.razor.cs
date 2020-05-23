using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using Microsoft.AspNetCore.Components;

namespace ExchangeAdvisor.SignalRClient.Shared
{
    public partial class RateHistoryAndForecast : ComponentBase
    {
        protected override async Task OnInitializedAsync()
        {
            await RefreshDataAsync();
        }

        private async Task RefreshDataAsync()
        {
            await RateService.RefreshSavedDataIfNeed(CurrencyPair);
            await Task.WhenAll(
                FetchHistoryAndActualForecastShowingLoaderAsync(),
                FetchForecastsMetadataAsync());
        }

        private async Task FetchHistoryAndActualForecastShowingLoaderAsync()
        {
            ShouldChartShowLoader = true;
            try
            {
                await Task.WhenAll(FetchHistoryAsync(), FetchActualForecastAsync());
            }
            finally
            {
                ShouldChartShowLoader = false;
            }
        }

        private async Task FetchHistoryAsync()
        {
            History = await RateService.GetHistoryAsync(CurrencyPair);
        }

        private async Task FetchActualForecastAsync()
        {
            ActualForecast = await RateService.GetNewestForecastAsync(CurrencyPair);
        }

        private async Task FetchForecastsMetadataAsync()
        {
            var forecastsMetadata = await RateService.GetForecastsMetadataAsync(CurrencyPair);

            ForecastsMetadata = forecastsMetadata.OrderBy(m => m.CreationDay).ToArray();

            foreach (var a in ForecastsMetadata) // TODO: allow edit description
                a.Description = "Some description";
        }

        // TODO: try to use only fields instead of private properties
        private CurrencyPair CurrencyPair { get; set; } = new CurrencyPair(Currency.USD, Currency.PLN);
        
        private RateHistory History { get; set; }
        
        private RateForecast ActualForecast { get; set; }

        private IReadOnlyCollection<RateForecast> OldForecasts { get; set; }

        private ICollection<RateForecastMetadata> ForecastsMetadata { get; set; }
        
        private DateTime? StartDate { get; set; } = DateTime.Today.AddMonths(-3);
        
        private DateTime? EndDate { get; set; } = DateTime.Today.AddMonths(3);
        
        private bool ShouldChartShowMarkers { get; set; } = false;
        
        private bool ShouldChartShowLoader { get; set; } = true;

        [Inject]
        private IRateService RateService { get; set; }
    }
}
