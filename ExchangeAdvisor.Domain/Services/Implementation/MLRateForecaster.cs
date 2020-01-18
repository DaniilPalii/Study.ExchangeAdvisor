using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.ML.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeAdvisor.Domain.Services.Implementation
{
    public class MLRateForecaster : IMLRateForecaster
    {
        public IEnumerable<Rate> Forecast(
            CurrencySymbol baseCurrency,
            CurrencySymbol comparingCurrency,
            DateTime forecastStartDay,
            DateTime forecastFinishDay)
        {
            var inputs = GenerateModelInputs(baseCurrency, comparingCurrency, forecastStartDay, forecastFinishDay);

            return ConsumeModel.Predict(inputs)
                .Select(ToRate);
        }

        private static IEnumerable<ModelInput> GenerateModelInputs(
            CurrencySymbol baseCurrency,
            CurrencySymbol comparingCurrency,
            DateTime forecastStartDay,
            DateTime forecastFinishDay)
        {
            for (var day = forecastStartDay; day < forecastFinishDay; day = day.AddDays(1))
            {
                yield return new ModelInput
                {
                    BaseCurrency = baseCurrency.ToString(),
                    ComparingCurrency = comparingCurrency.ToString(),
                    Day = day.Day,
                    Month = day.Month,
                    Year = day.Year,
                    AbsoluteDayNumber = (float)(day - DateTime.MinValue).TotalDays,
                    DayOfTear = day.DayOfYear,
                    DayOfWeek = day.DayOfWeek.ToString()
                };
            }
        }

        private static Rate ToRate((ModelInput, ModelOutput) ioPair)
        {
            var (input, output) = ioPair;

            return new Rate(
                day: new DateTime((int)input.Year, (int)input.Month, (int)input.Day),
                value: output.Score,
                baseCurrency: Converter.ToCurrencySymbol(input.BaseCurrency),
                comparingCurrency: Converter.ToCurrencySymbol(input.ComparingCurrency));
        }
    }
}
