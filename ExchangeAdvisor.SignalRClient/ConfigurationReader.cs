using Microsoft.Extensions.Configuration;

namespace ExchangeAdvisor.SignalRClient
{
    public class ConfigurationReader
    {
        public ConfigurationReader(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string SyncfusionLicenseKey => configuration.GetValue<string>("SyncfusionLicenseKey");

        public string DatabaseConnectionString => configuration.GetConnectionString("ExchangeAdvisor");

        private readonly IConfiguration configuration;
    }
}
