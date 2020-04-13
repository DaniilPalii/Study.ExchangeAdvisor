using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Spinner;

namespace ExchangeAdvisor.SignalRClient.Shared
{
    public partial class RateHistoryAndPrediction : ComponentBase
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
                FetchSavedForecastsMetadataAsync());
        }

        private async Task FetchHistoryAndActualForecastShowingLoaderAsync()
        {
            ShowChartLoader();
            try
            {
                await Task.WhenAll(FetchHistoricalRatesAsync(), FetchActualForecastRatesAsync());
            }
            finally
            {
                HideChartLoader();
            }
        }

        private async Task FetchHistoricalRatesAsync()
        {
            var history = await RateService.GetHistoryAsync(CurrencyPair);

            HistoricalRates = history.Rates.OrderBy(r => r.Day).ToArray();
        }

        private async Task FetchActualForecastRatesAsync()
        {
            var forecast = await RateService.GetNewestForecastAsync(CurrencyPair);

            ForecastRates = forecast.Rates.OrderBy(r => r.Day).ToArray();
        }

        private async Task FetchSavedForecastsMetadataAsync()
        {
            var forecastsMetadata = await RateService.GetForecastsMetadataAsync(CurrencyPair);

            SavedForecastsMetadata = forecastsMetadata.OrderBy(m => m.CreationDay).ToArray();

            foreach (var a in SavedForecastsMetadata) // TODO: allow edit description
                a.Description = "Some description";
        }

        private void ShowChartLoader() // TODO: move chart to separate component
        {
            ChartLoader.ShowSpinner(ChartLoaderTargetCssSelector);
        }

        private void HideChartLoader()
        {
            ChartLoader.HideSpinner(ChartLoaderTargetCssSelector);
        }

        private CurrencyPair CurrencyPair { get; set; } = new CurrencyPair(Currency.USD, Currency.PLN);
        private IReadOnlyCollection<Rate> HistoricalRates { get; set; } = Array.Empty<Rate>();
        private IReadOnlyCollection<Rate> ForecastRates { get; set; } = Array.Empty<Rate>();
        private IReadOnlyCollection<RateForecastMetadata> SavedForecastsMetadata { get; set; }
            = Array.Empty<RateForecastMetadata>();
        private DateTime? StartDate { get; set; } = DateTime.Today.AddMonths(-3);
        private DateTime? EndDate { get; set; } = DateTime.Today.AddMonths(3);
        private bool ShouldShowMarkers { get; set; } = false;
        private SfSpinner ChartLoader { get; set; } = new SfSpinner();

        [Inject]
        private IRateService RateService { get; set; }
        
        private static readonly string ChartLoaderTargetCssSelector = $"#{ChartId}";
        private const string ChartId = "chart";
    }
}
