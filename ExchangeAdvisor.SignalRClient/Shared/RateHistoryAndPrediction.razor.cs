using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Helpers;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Spinner;

namespace ExchangeAdvisor.SignalRClient.Shared
{
    public partial class RateHistoryAndPrediction : ComponentBase
    {
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await RefreshAsync();
        }

        private async Task RefreshAsync()
        {
            HistoricalRates = Array.Empty<Rate>();
            ForecastRates = Array.Empty<Rate>();

            ShowChartLoader();
            try
            {
                await Task.WhenAll(
                    FetchHistoricalRatesAsync(),
                    FetchActualForecastRatesAsync(),
                    FetchSavedForecastsMetadataAsync());
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
            var forecast = await RateService.GetActualForecastAsync(CurrencyPair);

            ForecastRates = forecast.Rates.OrderBy(r => r.Day).ToArray();
        }

        private async Task FetchSavedForecastsMetadataAsync()
        {
            var forecastsMetadata = await RateService.GetAllSavedForecastsMetadataAsync(CurrencyPair);

            SavedForecastsMetadata = forecastsMetadata.OrderBy(m => m.CreationDay).ToArray();

            foreach (var a in SavedForecastsMetadata)
                a.Description = "Some description";
        }

        private void ShowChartLoader()
        {
            ChartLoader.ShowSpinner(ChartLoaderTargetCssSelector);
        }

        private void HideChartLoader()
        {
            ChartLoader.HideSpinner(ChartLoaderTargetCssSelector);
        }

        private CurrencyPair CurrencyPair => new CurrencyPair(BaseCurrency, ComparingCurrency);
        private Currency BaseCurrency => Converter.ToCurrency(BaseCurrencyName);
        private Currency ComparingCurrency => Converter.ToCurrency(ComparingCurrencyName);
        private IReadOnlyCollection<Rate> HistoricalRates { get; set; } = Array.Empty<Rate>();
        private IReadOnlyCollection<Rate> ForecastRates { get; set; } = Array.Empty<Rate>();
        private IReadOnlyCollection<RateForecastMetadata> SavedForecastsMetadata { get; set; }
            = Array.Empty<RateForecastMetadata>();
        private DateTime? StartDate { get; set; } = DateTime.Today.AddMonths(-3);
        private DateTime? EndDate { get; set; } = DateTime.Today.AddMonths(3);
        private string BaseCurrencyName { get; set; } = Currency.EUR.ToString();
        private string ComparingCurrencyName { get; set; } = Currency.PLN.ToString();
        private bool ShouldShowMarkers { get; set; } = false;
        private SfSpinner ChartLoader { get; set; } = new SfSpinner();

        [Inject]
        private IRateService RateService { get; set; }

        private static readonly IReadOnlyCollection<string> Currencies = Enum.GetNames(typeof(Currency));
        private static readonly string ChartLoaderTargetCssSelector = $"#{ChartId}";
        private const string ChartId = "chart";
    }
}
