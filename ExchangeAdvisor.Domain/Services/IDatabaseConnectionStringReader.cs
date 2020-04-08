namespace ExchangeAdvisor.Domain.Services
{
    public interface IDatabaseConnectionStringReader
    {
        string DatabaseConnectionString { get; }
    }
}
