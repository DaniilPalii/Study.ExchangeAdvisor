using ExchangeAdvisor.Domain.Values;
using System;
using System.Collections.Generic;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IRateForecaster
    {
        IEnumerable<Rate> Forecast(
            CurrencySymbol baseCurrency,
            CurrencySymbol comparingCurrency,
            DateTime forecastStartDay,
            DateTime forecastFinishDay);
    }
}
