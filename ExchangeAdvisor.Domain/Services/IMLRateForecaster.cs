using ExchangeAdvisor.Domain.Values;
using System;
using System.Collections.Generic;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IMLRateForecaster
    {
        IEnumerable<Rate> Forecast(
            CurrencySymbol baseCurrency,
            CurrencySymbol comparingCurrency,
            DateTime forecastStartDay,
            DateTime forecastFinishDay);
    }
}
