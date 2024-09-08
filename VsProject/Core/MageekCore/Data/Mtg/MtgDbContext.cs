using Microsoft.EntityFrameworkCore;
using MageekCore.Data.Mtg.Entities;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MageekCore.Data.Mtg
{

    public class MtgDbContext : DbContext
    {

        readonly string dbPath;
        public MtgDbContext(string dbPath) { this.dbPath = dbPath; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = " + dbPath);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<CardForeignData> cardForeignData { get; set; }
        public DbSet<CardIdentifiers> cardIdentifiers { get; set; }
        public DbSet<CardLegalities> cardLegalities { get; set; }
        public DbSet<CardPurchaseUrls> cardPurchaseUrls { get; set; }
        public DbSet<CardRulings> cardRulings { get; set; }
        public DbSet<Cards> cards { get; set; }
        public DbSet<Meta> meta { get; set; }
        public DbSet<SetBoosterContents> setBoosterContents { get; set; }
        public DbSet<SetBoosterContentWeights> setBoosterContentWeights { get; set; }
        public DbSet<SetBoosterSheetCards> setBoosterSheetCards { get; set; }
        public DbSet<SetBoosterSheets> setBoosterSheets { get; set; }
        public DbSet<Sets> sets { get; set; }
        public DbSet<SetTranslations> setTranslations { get; set; }
        public DbSet<TokenIdentifiers> tokenIdentifiers { get; set; }
        public DbSet<Tokens> tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CardForeignData>().HasKey(m => new { m.Uuid, m.Language });
        }

    }

    //public class DesignTimeMtgDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
    //{
    //    public UsersDbContext CreateDbContext(string[] args)
    //    {
    //        var configuration = new ConfigurationBuilder()
    //            .SetBasePath(Directory.GetCurrentDirectory())
    //            .Build();
    //        var builder = new DbContextOptionsBuilder<UsersDbContext>();
    //        builder.UseSqlite(string.Concat("Data Source = ", dbPath));
    //        return new UsersDbContext(builder.Options);
    //    }
    //}

}
