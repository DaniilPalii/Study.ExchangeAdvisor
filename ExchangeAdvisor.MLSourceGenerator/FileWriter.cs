using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeAdvisor.ML.SourceGenerator
{
    public class FileWriter
    {
        public FileWriter(IRateFetcher rateFetcher)
        {
            this.rateFetcher = rateFetcher;
        }

        public async Task SaveAllExchangeRatesToTsvAsync(string filePath)
        {
            var rates = await FetchRates().ConfigureAwait(false);
            var fileContent = GenerateFileContent(rates);

            File.WriteAllText(filePath, fileContent);
        }

        private async Task<IEnumerable<Rate>> FetchRates()
        {
            var euroBasedRatesFetchingTask = rateFetcher.FetchRateHistoryAsync(DateTime.MinValue, DateTime.Today, CurrencySymbol.EUR);
            var dollarBasedRatesFetchingTask = rateFetcher.FetchRateHistoryAsync(DateTime.MinValue, DateTime.Today, CurrencySymbol.USD);
            var euroBasedRates = await euroBasedRatesFetchingTask.ConfigureAwait(false);
            var dollarBasedRates = await dollarBasedRatesFetchingTask.ConfigureAwait(false);

            return euroBasedRates.Concat(dollarBasedRates);
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
                    ("Absolute day number", r => (r.Day - DateTime.MinValue).TotalDays.ToString()),
                    ("Day of week", r => r.Day.DayOfWeek.ToString()),
                    ("Day of year", r => r.Day.DayOfYear.ToString()),
                    ("Rate", r => r.Value.ToString(CultureInfo.InvariantCulture.NumberFormat)),
                    ("Base currency", r => r.BaseCurrency.ToString()),
                    ("Comparing currency", r => r.ComparingCurrency.ToString())
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

        private readonly IRateFetcher rateFetcher;
    }
}
