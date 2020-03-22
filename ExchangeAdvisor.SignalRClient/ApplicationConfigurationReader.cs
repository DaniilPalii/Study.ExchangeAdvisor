using System;
using System.IO;
using System.Reflection;
using ExchangeAdvisor.Domain.Services;
using Microsoft.Extensions.Configuration;

namespace ExchangeAdvisor.SignalRClient
{
    public class ApplicationConfigurationReader : IConfigurationReader
    {
        public string SyncfusionLicenseKey => configuration.GetValue<string>("SyncfusionLicenseKey");

        public string DatabaseConnectionString => configuration.GetConnectionString("ExchangeAdvisor");

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
