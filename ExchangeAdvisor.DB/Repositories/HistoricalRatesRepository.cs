using System.Collections.Generic;
using System.Linq;
using ExchangeAdvisor.DB.Converters;
using ExchangeAdvisor.DB.Entities;
using ExchangeAdvisor.Domain.Services;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.DB.Repositories
{
    public class HistoricalRatesRepository : IHistoricalRatesRepository
    {
        public HistoricalRatesRepository()
        {
            entityRepository = new Repository<HistoricalRate>();
        }

        public IEnumerable<Rate> Get(DateRange dateRange, CurrencyPair currencyPair)
        {
            return entityRepository.GetBy(r => dateRange.DoesContain(r.Day)
                                            && r.BaseCurrency == currencyPair.Base
                                            && r.ComparingCurrency == currencyPair.Comparing)
                .Select(r => r.ToRate());
        }

        public void Update(IEnumerable<Rate> rates)
        {
            foreach (var ratesByCurrecyPair in rates.GroupBy(e => e.CurrencyPair))
            {
                var currencyPair = ratesByCurrecyPair.Key;
                var existingRates = entityRepository
                    .GetBy(r => r.BaseCurrency == currencyPair.Base && r.ComparingCurrency == currencyPair.Comparing)
                    .Select(r => r.ToRate())
                    .ToHashSet();
                var newHistoricalRates = ratesByCurrecyPair.Where(r => !existingRates.Contains(r))
                    .Select(r => r.ToHistoricalRate());

                entityRepository.Update(newHistoricalRates);
            }
        }

        private readonly Repository<HistoricalRate> entityRepository;
    }
}
