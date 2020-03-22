using ExchangeAdvisor.DB.Context;
using Microsoft.EntityFrameworkCore;

namespace ExchangeAdvisor.DB.Migrations
{
    public class DatabaseMigrator
    {
        public DatabaseMigrator(string databaseConnectionString)
        {
            this.databaseConnectionString = databaseConnectionString;
        }

        public void Migrate()
        {
            using var databaseContext = new DatabaseContext(databaseConnectionString);

            databaseContext.Database.Migrate();
        }

        private readonly string databaseConnectionString;
    }
}
