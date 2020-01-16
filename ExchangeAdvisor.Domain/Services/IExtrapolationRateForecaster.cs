using System;
using System.Collections.Generic;
using ExchangeAdvisor.Domain.Services.Implementation;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IExtrapolationRateForecaster
    {
        IEnumerable<Rate> Forecast(
            IReadOnlyCollection<Rate> source,
            DateTime forecastFinishDay,
            ForecastMethod forecastMethod);

        IEnumerable<Rate> ForecastOnKnownAndUnknownRange(
            IReadOnlyCollection<Rate> source, 
            DateTime forecastFinishDay,
            ForecastMethod forecastMethod);
    }
}