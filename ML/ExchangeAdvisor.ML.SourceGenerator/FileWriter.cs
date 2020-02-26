using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using ExchangeAdvisor.ML.Model;

namespace ExchangeAdvisor.ML.SourceGenerator
{
    public class FileWriter
    {
        public FileWriter(IRateWebFetcher rateFetcher)
        {
            this.rateFetcher = rateFetcher;
        }

        public void SaveAllExchangeRatesToTsv(string filePath)
        {
            var rates = FetchRates();
            var fileContent = GenerateFileContent(rates);

            File.WriteAllText(filePath, fileContent);
        }

        private IEnumerable<Rate> FetchRates()
        {
            return WaitForAll(
                rateFetcher.FetchAsync(
                    new DateRange(
                        DateTime.MinValue,
                        DateTime.Today),
                    new CurrencyPair(Currency.EUR, Currency.PLN)));
        }

        private static string GenerateFileContent(IEnumerable<Rate> rates)
        {
            return GenerateFileContent(
                rates,
                new (string featureName, Func<Rate, string> toFeatureValue)[]
                {
                    ("Year", r => r.Day.Year.ToString()),
                    ("Month", r => r.Day.Month.ToString()),
                    ("Day", r => r.Day.Day.ToString()),
                    ("Absolute day number", r => ModelInput.GetAbsoluteDayNumber(r.Day).ToString()),
                    ("Day of week", r => ModelInput.GetDayOfWeekNumber(r.Day).ToString()),
                    ("Day of year", r => r.Day.DayOfYear.ToString()),
                    ("Rate", r => r.Value.ToString(CultureInfo.InvariantCulture.NumberFormat)),
                    ("Base currency", r => r.CurrencyPair.Base.ToString()),
                    ("Comparing currency", r => r.CurrencyPair.Comparing.ToString())
                });
        }

        private static string GenerateFileContent(
            IEnumerable<Rate> rates,
            IReadOnlyCollection<(string featureName, Func<Rate, string> toFeatureValue)> features)
        {
            var stringBuilder = new StringBuilder()
                .AppendJoin('\t', features.Select(f => f.featureName))
                .Append(Environment.NewLine);

            foreach (var rate in rates)
                stringBuilder.AppendJoin('\t', features.Select(f => f.toFeatureValue(rate)))
                    .Append(Environment.NewLine);

            return stringBuilder.ToString();
        }

        private static IEnumerable<T> WaitForAll<T>(params Task<IEnumerable<T>>[] tasks)
        {
            Task.WaitAll(tasks);

            return tasks.SelectMany(t => t.Result);
        }

        private readonly IRateWebFetcher rateFetcher;
    }
}
