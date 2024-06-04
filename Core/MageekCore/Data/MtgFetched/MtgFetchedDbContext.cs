using Microsoft.EntityFrameworkCore;
using MageekCore.Data.Collection.Entities;
using MageekCore.Data.MtgFetched.Entities;

namespace MageekCore.Data.MtgFetched
{

    public class MtgFetchedDbContext : DbContext
    {

        readonly string dbPath;
        public MtgFetchedDbContext(string dbPath) { this.dbPath = dbPath; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = " + dbPath);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<ArchetypeCard> CardArchetypes { get; set; }
        public DbSet<CardTraduction> CardTraductions { get; set; }
        public DbSet<PriceLine> PriceLine { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CardTraduction>().HasKey(m => new { m.CardUuid, m.Language });
        }

    }

}
