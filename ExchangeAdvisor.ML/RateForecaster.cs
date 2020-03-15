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
        public async Task<IEnumerable<Rate>> ForecastAsync(DateRange dateRange, CurrencyPair currencyPair)
        {
            var modelBuildingTask = Task.Run(() => new ModelBuilder().Build());
            var inputs = dateRange.Days.Select(d => new ModelInput(d));
            var model = await modelBuildingTask;

            return model.Predict(inputs)
                .Select(p => ToRate(p, currencyPair));
        }

        private static Rate ToRate((ModelInput, ModelOutput) inputOutputModelsPair, CurrencyPair currencyPair)
        {
            var (input, output) = inputOutputModelsPair;

            return new Rate(
                day: new DateTime((int)input.Year, (int)input.Month, (int)input.Day),
                value: output.Score,
                currencyPair);
        }
    }
}
