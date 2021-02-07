using System.Reflection;
using ExchangeAdvisor.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeAdvisor.DB.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<RateHistoryEntity> RateHistories { get; set; }

        public DbSet<RateForecastEntity> RateForecasts { get; set; }

        public DbSet<HistoricalRateEntity> HistoricalRates { get; set; }

        public DbSet<ForecastedRateEntity> ForecastedRates { get; set; }

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
