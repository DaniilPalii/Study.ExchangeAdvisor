using ExchangeAdvisor.DB.Context;
using ExchangeAdvisor.SignalRClient;
using Microsoft.EntityFrameworkCore.Design;

namespace ExchangeAdvisor.DBMigrationGenerator
{
    public class DesignTimeDatabaseContextBuilder : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var databaseConnectionString = new ApplicationConfigurationReader().DatabaseConnectionString;

            return new DatabaseContext(databaseConnectionString);
        }
    }
}
