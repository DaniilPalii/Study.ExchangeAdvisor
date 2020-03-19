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

        private readonly IConfiguration configuration;
    }
}
