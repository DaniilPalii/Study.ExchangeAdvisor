using System;
using System.ComponentModel.DataAnnotations.Schema;
using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.DB.Entities
{
    [Table(name: "ForecastedRate")]
    public class ForecastedRateEntity : EntityBase
    {
        public DateTime Day { get; set; }

        public float Value { get; set; }
        
        public RateForecastEntity Forecast { get; set; }

        public ForecastedRateEntity() { }

        public ForecastedRateEntity(Rate rate, RateForecastEntity rateForecastEntity) : this(rate)
        {
            Forecast = rateForecastEntity;
        }

        public ForecastedRateEntity(Rate rate)
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
