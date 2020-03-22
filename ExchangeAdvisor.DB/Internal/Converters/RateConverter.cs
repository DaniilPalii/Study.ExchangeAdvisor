using ExchangeAdvisor.DB.Entities;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.DB.Internal.Converters
{
    internal static class RateConverter
    {
        public static HistoricalRate ToHistoricalRate(this Rate rate)
        {
            return new HistoricalRate
            {
                Day = rate.Day,
                BaseCurrency = rate.CurrencyPair.Base,
                ComparingCurrency = rate.CurrencyPair.Comparing,
                Value = rate.Value
            };
        }

        public static Rate ToRate(this HistoricalRate historicalRate)
        {
            return new Rate(
                historicalRate.Day,
                historicalRate.Value,
                new CurrencyPair(historicalRate.BaseCurrency, historicalRate.ComparingCurrency));
        }
    }
}
