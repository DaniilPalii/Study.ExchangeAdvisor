namespace ExchangeAdvisor.Domain.Services
{
    public interface IConfigurationReader
    {
        string DatabaseConnectionString { get; }
    }
}
