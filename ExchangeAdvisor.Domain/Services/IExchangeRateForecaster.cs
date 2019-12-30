using System;
using System.Collections.Generic;
using ExchangeAdvisor.Domain.Services.Implementation;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IExchangeRateForecaster
    {
        IEnumerable<RateOnDay> Forecast(
            IReadOnlyCollection<RateOnDay> source,
            DateTime forecastFinishDay,
            ForecastMethod forecastMethod);

        IEnumerable<RateOnDay> ForecastOnKnownAndUnknownRange(
            IReadOnlyCollection<RateOnDay> source, 
            DateTime forecastFinishDay,
            ForecastMethod forecastMethod);
    }
}