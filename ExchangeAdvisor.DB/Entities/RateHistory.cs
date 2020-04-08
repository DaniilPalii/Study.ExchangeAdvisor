using System.Linq;
using ExchangeAdvisor.Domain.Values;
using DomainRateHistory = ExchangeAdvisor.Domain.Values.Rate.RateHistory;

namespace ExchangeAdvisor.DB.Entities
{
    public class RateHistory : RateCollectionEntityBase
    {
        public DomainRateHistory ToDomain()
        {
            return new DomainRateHistory(
                Rates.Select(r => r.ToDomain()),
                new CurrencyPair(BaseCurrency, ComparingCurrency));
        }
    }
}