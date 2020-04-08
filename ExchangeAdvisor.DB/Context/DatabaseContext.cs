using System.Reflection;
using ExchangeAdvisor.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeAdvisor.DB.Context
{
    public class DatabaseContext : DbContext
    {
        // TODO: use plural names if possible
        public DbSet<RateHistoryEntity> RateHistory { get; set; }

        public DbSet<RateForecastEntity> RateForecast { get; set; }

        public DbSet<HistoricalRateEntity> HistoricalRate { get; set; }
        
        public DbSet<ForecastRateEntity> ForecastRate { get; set; }

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
