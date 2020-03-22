using System.Reflection;
using ExchangeAdvisor.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeAdvisor.DB.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DbSet<HistoricalRate> HistoricalRates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var thisAssemblyName = Assembly.GetAssembly(typeof(DatabaseContext)).GetName().Name;

            optionsBuilder.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly(thisAssemblyName));
        }

        private readonly string connectionString;
    }
}
