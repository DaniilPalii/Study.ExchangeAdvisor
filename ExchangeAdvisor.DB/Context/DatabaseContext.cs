using ExchangeAdvisor.DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeAdvisor.DB.Context
{
    public class DatabaseContext : DbContext
    {
        public DbSet<HistoricalRate> HistoricalRates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=ExchangeAdvisor;Integrated Security=True");

        // TODO: move connection string to configuration file in this assembly
    }
}
