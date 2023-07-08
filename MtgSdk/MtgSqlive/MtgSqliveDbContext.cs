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

        public DbSet<CardForeignData> CardForeignData { get; set; }
        public DbSet<CardIdentifiers> CardIdentifiers { get; set; }
        public DbSet<CardLegalities> CardLegalities { get; set; }
        public DbSet<CardPurchaseUrls> CardPurchaseUrls { get; set; }
        public DbSet<CardRulings> CardRulings { get; set; }
        public DbSet<Cards> Cards { get; set; }
        public DbSet<Meta> Meta { get; set; }
        public DbSet<SetBoosterContents> SetBoosterContents { get; set; }
        public DbSet<SetBoosterContentWeights> SetBoosterContentWeights { get; set; }
        public DbSet<SetBoosterSheetCards> SetBoosterSheetCards { get; set; }
        public DbSet<SetBoosterSheets> SetBoosterSheets { get; set; }
        public DbSet<Sets> Sets { get; set; }
        public DbSet<SetTranslations> SetTranslations { get; set; }
        public DbSet<TokenIdentifiers> TokenIdentifiers { get; set; }
        public DbSet<Tokens> Tokens { get; set; }

    }

}
