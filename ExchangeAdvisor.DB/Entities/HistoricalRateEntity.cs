using System;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.DB.Entities
{
    public class HistoricalRateEntity : EntityBase
    {
        public DateTime Day { get; set; }

        public float Value { get; set; }
        
        public RateHistoryEntity History { get; set; }

        public HistoricalRateEntity() { }

        public HistoricalRateEntity(Rate rate, RateHistoryEntity rateHistory)
        {
            Day = rate.Day;
            Value = rate.Value;
            History = rateHistory;
        }

        public Rate ToRate()
        {
            return new Rate(Day, Value);
        }
    }
}