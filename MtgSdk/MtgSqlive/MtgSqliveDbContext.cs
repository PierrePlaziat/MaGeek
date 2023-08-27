using MageekSdk.MtgSqlive.Entities;
using Microsoft.EntityFrameworkCore;

namespace MageekSdk.MtgSqlive
{

    public class MtgSqliveDbContext : DbContext
    {

        readonly string dbPath;
        public MtgSqliveDbContext(string dbPath) { this.dbPath = dbPath; }

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

}
