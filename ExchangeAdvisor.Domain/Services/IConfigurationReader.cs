using System;

namespace ExchangeAdvisor.Domain.Services
{
    public interface IConfigurationReader
    {
        TimeSpan ForecastingOffset { get; }
    }
}
