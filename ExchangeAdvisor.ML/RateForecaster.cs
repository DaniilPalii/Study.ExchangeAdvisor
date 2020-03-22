using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.ML.Internal;

namespace ExchangeAdvisor.ML
{
    public class RateForecaster : IRateForecaster
    {
        public RateForecaster()
        {
            modelBuilder = new ModelBuilder();
        }

        public async Task<IEnumerable<Rate>> ForecastAsync(ICollection<Rate> historicalRates, DateRange dateRange)
        {
            var modelBuildingTask = Task.Run(() => modelBuilder.Build(historicalRates));
            var inputs = dateRange.Days.Select(d => new ModelPredictionInput(d));

            var model = await modelBuildingTask;
            var prediction = model.Predict(inputs);

            var currencyPair = historicalRates.First().CurrencyPair;
            return prediction.Select(p => ToRate(p, currencyPair));
        }

        private static Rate ToRate((ModelPredictionInput, ModelOutput) inputOutputModelsPair, CurrencyPair currencyPair)
        {
            var (input, output) = inputOutputModelsPair;

            return new Rate(
                day: new DateTime((int)input.Year, (int)input.Month, (int)input.Day),
                value: output.Score,
                currencyPair);
        }

        private readonly ModelBuilder modelBuilder;
    }
}
