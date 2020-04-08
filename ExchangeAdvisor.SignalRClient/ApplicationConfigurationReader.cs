using System;
using ExchangeAdvisor.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace ExchangeAdvisor.SignalRClient
{
    public class ApplicationConfigurationReader : IConfigurationReader, IDatabaseConnectionStringReader
    {
        public string SyncfusionLicenseKey => configuration.GetValue<string>("SyncfusionLicenseKey");

        public string DatabaseConnectionString => configuration.GetConnectionString("ExchangeAdvisor");

        public TimeSpan ForecastingOffset
        {
            get
            {
                var forecastingOffsetInDays = configuration.GetValue<int>("ForecastingOffsetInDays");

                return new TimeSpan(forecastingOffsetInDays, hours: 0, minutes: 0, seconds: 0);
            }
        }

        public ApplicationConfigurationReader(IConfiguration applicationConfiguration)
        {
            configuration = applicationConfiguration;
        }

        public ApplicationConfigurationReader()
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
        }

        private readonly IConfiguration configuration;
    }
}
