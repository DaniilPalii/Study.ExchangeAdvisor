using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.DB.Entities
{
    public class HistoricalRate : RateEntityBase
    {
        public RateHistory History { get; set; }

        public HistoricalRate() { }

        public HistoricalRate(Rate domainRate, RateHistory rateHistory) : base(domainRate)
        {
            History = rateHistory;
        }
    }
}