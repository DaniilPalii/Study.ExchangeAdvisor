using System;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.DB.Entities
{
    public class ForecastRateEntity : EntityBase
    {
        public DateTime Day { get; set; }

        public float Value { get; set; }
        
        public RateForecastEntity Forecast { get; set; }

        public ForecastRateEntity() { }

        public ForecastRateEntity(Rate rate, RateForecastEntity rateForecastEntity) : this(rate)
        {
            Forecast = rateForecastEntity;
        }

        public ForecastRateEntity(Rate rate)
        {
            Day = rate.Day;
            Value = rate.Value;
        }

        public Rate ToRate()
        {
            return new Rate(Day, Value);
        }
    }
}
