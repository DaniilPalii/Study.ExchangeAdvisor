using ExchangeAdvisor.DB.Context;
using ExchangeAdvisor.Domain.Services;

namespace ExchangeAdvisor.DB.Repositories
{
    public abstract class RepositoryBase
    {
        protected RepositoryBase(IDatabaseConnectionStringReader connectionStringReader)
        {
            databaseConnectionString = connectionStringReader.DatabaseConnectionString;
        }

        protected DatabaseContext CreateDatabaseContext()
        {
            return new DatabaseContext(databaseConnectionString);
        }

        private readonly string databaseConnectionString;
    }
}