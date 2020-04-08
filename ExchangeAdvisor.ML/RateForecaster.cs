using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.Domain.Values.Rate;
using ExchangeAdvisor.ML.Internal;

namespace ExchangeAdvisor.ML
{
    public class RateForecaster : IRateForecaster
    {
        public RateForecaster()
        {
            modelBuilder = new ModelBuilder();
        }

        public async Task<RateForecast> ForecastAsync(RateHistory history, DateRange dateRange)
        {
            var modelBuildingTask = Task.Run(() => modelBuilder.Build(history));
            var inputs = dateRange.Days.Select(d => new ModelPredictionInput(d));

            var model = await modelBuildingTask;
            var prediction = model.Predict(inputs);

            return ToRateForecast(prediction, history.CurrencyPair);
        }

        private static RateForecast ToRateForecast(
            IEnumerable<(ModelPredictionInput, ModelOutput)> prediction,
            CurrencyPair currencyPair)
        {
            var rates = prediction.Select(ToRate);

            return new RateForecast(rates, currencyPair, DateTime.Today);
        }

        private static Rate ToRate((ModelPredictionInput, ModelOutput) inputOutputModelsPair)
        {
            var (input, output) = inputOutputModelsPair;
            var day = new DateTime((int) input.Year, (int) input.Month, (int) input.Day);

            return new Rate(day, output.Score);
        }

        private readonly ModelBuilder modelBuilder;
    }
}