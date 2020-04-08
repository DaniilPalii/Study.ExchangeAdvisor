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

        public ForecastRateEntity(Rate rate, RateForecastEntity rateForecastEntity)
        {
            Day = rate.Day;
            Value = rate.Value;
            Forecast = rateForecastEntity;
        }

        public Rate ToRate()
        {
            return new Rate(Day, Value);
        }
    }
}
