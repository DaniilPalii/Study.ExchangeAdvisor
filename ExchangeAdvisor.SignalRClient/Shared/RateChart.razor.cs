using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ExchangeAdvisor.Domain.Extensions;
using ExchangeAdvisor.Domain.Values.Rate;
using ExchangeAdvisor.SignalRClient.ViewModels;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Spinner;

namespace ExchangeAdvisor.SignalRClient.Shared
{
    public partial class RateChart : ComponentBase
    {
        [Parameter]
        public IReadOnlyCollection<ChartSeriesViewModel<Rate>> RateChartSeries
        {
            get => rateChartSeriesViewModels;
            set
            {
                rateChartSeriesViewModels = value;
                
                if (rateChartSeriesViewModels != null)
                    GenerateColorsForChartSeriesIfLack();
            }
        }

        [Parameter]
        public bool ShouldShowMarkers { get; set; }

        [Parameter]
        public bool ShouldShowLoader
        {
            get => shouldShowLoader;
            set
            {
                shouldShowLoader = value;
                
                if (shouldShowLoader)
                    loader.ShowSpinner(LoaderTargetCssSelector);
                else
                    loader.HideSpinner(LoaderTargetCssSelector);
            }
        }

        private void GenerateColorsForChartSeriesIfLack()
        {
            var seriesWithoutColor = rateChartSeriesViewModels.Where(m => string.IsNullOrEmpty(m.ColorHexCode));

            foreach (var (series, color) in seriesWithoutColor.Zip(PossibleColors.RepeatEndlessly()))
                series.ColorHexCode = color.ToHexCode();
        }

        private IReadOnlyCollection<ChartSeriesViewModel<Rate>> rateChartSeriesViewModels;
        private SfSpinner loader = new SfSpinner();
        private bool shouldShowLoader;

        private static readonly IReadOnlyCollection<Color> PossibleColors = new[]
        {
            Color.Blue,
            Color.Green,
            Color.Orange,
            Color.Purple,
            Color.Aqua,
            Color.Red,
            Color.Chocolate,
            Color.Lime
        };
        private static readonly string LoaderTargetCssSelector = $"#{Id}";
        private const string Id = "chart";
    }
}