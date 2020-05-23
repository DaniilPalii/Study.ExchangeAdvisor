using System.Collections.Generic;
using System.Linq;
using ExchangeAdvisor.Domain.Extensions;
using ExchangeAdvisor.Domain.Values.Rate;
using ExchangeAdvisor.SignalRClient.ViewModels;
using Microsoft.AspNetCore.Components;

namespace ExchangeAdvisor.SignalRClient.Shared
{
    public partial class RateHistoryAndForecastChart : ComponentBase
    {
        [Parameter]
        public RateHistory History
        {
            get => history;
            set
            {
                history = value;
                chartSeriesViewModels[0] = new ChartSeriesViewModel<Rate>(HistoryTitle, history?.Rates);
            }
        }

        [Parameter]
        public RateForecast ActualForecast
        {
            get => actualForecast;
            set
            {
                actualForecast = value;
                chartSeriesViewModels[1] = new ChartSeriesViewModel<Rate>(ActualForecastTitle, actualForecast?.Rates);
            }
        }

        [Parameter]
        public IReadOnlyCollection<RateForecast> OldForecasts
        {
            get => oldForecasts;
            set
            {
                oldForecasts = value;
                chartSeriesViewModels.RemoveFromIndexToEnd(2);
                
                if (oldForecasts != null)
                    chartSeriesViewModels.AddRange(
                        oldForecasts.Select(f => new ChartSeriesViewModel<Rate>(GetTitleForOldForecast(f), f.Rates)));
            }
        }

        [Parameter]
        public bool ShouldShowMarkers { get; set; }

        [Parameter]
        public bool ShouldShowLoader { get; set; }
        
        private static string GetTitleForOldForecast(RateForecast forecast)
        {
            var forecastTitle = $"Forecast of {forecast.CreationDay:d}";

            return string.IsNullOrWhiteSpace(forecast.Description)
                ? forecastTitle
                : $"{forecastTitle} - {forecast.Description}";
        }

        private readonly List<ChartSeriesViewModel<Rate>> chartSeriesViewModels = new List<ChartSeriesViewModel<Rate>>
        {
            new ChartSeriesViewModel<Rate>(HistoryTitle), new ChartSeriesViewModel<Rate>(DefaultActualForecastTitle)
        };

        private string ActualForecastTitle
            => OldForecasts?.Any() == true ? "Actual forecast" : DefaultActualForecastTitle;

        private RateHistory history;
        private RateForecast actualForecast;
        private IReadOnlyCollection<RateForecast> oldForecasts = new RateForecast[] { };

        private const string HistoryTitle = "History";
        private const string DefaultActualForecastTitle = "Forecast";
    }
}