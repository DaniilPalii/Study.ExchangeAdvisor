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
        public RateForecaster(IHistoricalRatesRepository historicalRatesRepository)
        {
            modelBuilder = new ModelBuilder(historicalRatesRepository);
        }

        public async Task<IEnumerable<Rate>> ForecastAsync(DateRange dateRange, CurrencyPair currencyPair)
        {
            var modelBuildingTask = Task.Run(() => modelBuilder.Build(currencyPair));
            var inputs = dateRange.Days.Select(d => new ModelPredictionInput(d));
            var model = await modelBuildingTask;

            return model.Predict(inputs)
                .Select(p => ToRate(p, currencyPair));
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
