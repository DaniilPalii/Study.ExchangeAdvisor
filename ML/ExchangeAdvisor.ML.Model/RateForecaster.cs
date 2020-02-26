using System;
using System.Collections.Generic;
using System.Linq;
using ExchangeAdvisor.Domain.Helpers;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.ML.Model
{
    public class RateForecaster : IRateForecaster
    {
        public IEnumerable<Rate> Forecast(DateRange dateRange, CurrencyPair currencyPair)
        {
            var inputs = dateRange.Days.Select(d => new ModelInput(d, currencyPair));

            return ConsumeModel.Predict(inputs)
                .Select(ToRate);
        }

        private static Rate ToRate((ModelInput, ModelOutput) inputOutputModelsPair)
        {
            var (input, output) = inputOutputModelsPair;

            return new Rate(
                day: new DateTime((int)input.Year, (int)input.Month, (int)input.Day),
                value: output.Score,
                new CurrencyPair(
                    Converter.ToCurrency(input.BaseCurrency),
                    Converter.ToCurrency(input.ComparingCurrency)));
        }
    }
}
