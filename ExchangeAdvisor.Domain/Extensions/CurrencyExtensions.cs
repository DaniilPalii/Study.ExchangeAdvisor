using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Extensions
{
    public class CurrencyExtensions
    {
        public static Currency ToCurrency(string currencySymbol)
        {
            return EnumExtensions.ToEnum<Currency>(currencySymbol);
        }
    }
}