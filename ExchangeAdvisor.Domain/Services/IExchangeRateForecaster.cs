using System;
using System.Collections.Generic;
using ExchangeAdvisor.Domain.Values;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IExchangeRateForecaster
    {
        IEnumerable<RateOnDay> Forecast(IReadOnlyCollection<RateOnDay> source, DateTime predictionFinishDay);
    }
}