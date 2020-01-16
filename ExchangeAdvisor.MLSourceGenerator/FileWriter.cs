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
            var stringBuilder = new StringBuilder("Year\tMonth\tDay\tRate\tBase currency\tComparing currency\n");
            foreach (var rate in rates)
            {
                stringBuilder
                    .Append(rate.Day.Year).Append("\t")
                    .Append(rate.Day.Month).Append("\t")
                    .Append(rate.Day.Day).Append("\t")
                    .Append(rate.Value.ToString(CultureInfo.InvariantCulture.NumberFormat)).Append("\t")
                    .Append(rate.BaseCurrency).Append("\t")
                    .Append(rate.ComparingCurrency).Append("\n");
            }

            return stringBuilder.ToString();
        }

        private readonly IRateFetcher rateFetcher;
    }
}
