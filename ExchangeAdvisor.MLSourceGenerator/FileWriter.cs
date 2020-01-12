using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeAdvisor.MLSourceGenerator
{
    public class FileWriter
    {
        public FileWriter(IExchangeRateFetcher exchangeRateFetcher)
        {
            this.exchangeRateFetcher = exchangeRateFetcher;
        }

        public async Task SaveAllExchangeRatesToTsvAsync(string filePath)
        {
            var euroBasedRatesFetchingTask = exchangeRateFetcher.FetchRateHistoryAsync(
                DateTime.MinValue,
                DateTime.Today,
                CurrencySymbol.EUR);
            var dollarBasedRatesFetchingTask = exchangeRateFetcher.FetchRateHistoryAsync(
                DateTime.MinValue,
                DateTime.Today,
                CurrencySymbol.USD);
            var euroBasedRates = await euroBasedRatesFetchingTask.ConfigureAwait(false);
            var dollarBasedRates = await dollarBasedRatesFetchingTask.ConfigureAwait(false);
            var rates = euroBasedRates.Concat(dollarBasedRates);

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
            var fileContent = stringBuilder.ToString();

            File.WriteAllText(filePath, fileContent);
        }

        private readonly IExchangeRateFetcher exchangeRateFetcher;
    }
}
