using ExchangeAdvisor.Domain.Values.Rate;

namespace ExchangeAdvisor.DB.Entities
{
    public class ForecastRate : RateEntityBase
    {
        public RateForecast Forecast { get; set; }

        public ForecastRate() { }

        public ForecastRate(Rate domainRate, RateForecast rateForecast) : base(domainRate)
        {
            Forecast = rateForecast;
        }
    }
}
