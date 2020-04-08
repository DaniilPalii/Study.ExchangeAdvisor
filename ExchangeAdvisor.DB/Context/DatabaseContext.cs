using System.Reflection;
using ExchangeAdvisor.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeAdvisor.DB.Context
{
    public class DatabaseContext : DbContext
    {
        // TODO: use plural names if possible
        public DbSet<RateHistory> RateHistory { get; set; }

        public DbSet<RateForecast> RateForecast { get; set; }

        public DbSet<HistoricalRate> HistoricalRate { get; set; }
        
        public DbSet<ForecastRate> ForecastRate { get; set; }

        public DatabaseContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

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
