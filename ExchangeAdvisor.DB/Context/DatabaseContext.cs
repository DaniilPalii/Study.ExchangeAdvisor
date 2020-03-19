using ExchangeAdvisor.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeAdvisor.DB.Context
{
    internal class DatabaseContext : DbContext
    {
        public DatabaseContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DbSet<HistoricalRate> HistoricalRates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(connectionString);

        private readonly string connectionString;
    }
}
