using Microsoft.EntityFrameworkCore;
using MageekSdk.Collection.Entities;

namespace MageekSdk.Collection
{

    public class CollectionDbContext : DbContext
    {

        readonly string dbPath;
        public CollectionDbContext(string dbPath) { this.dbPath = dbPath; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = " + dbPath);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<ArchetypeCard> CardArchetypes { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<CardTraduction> CardTraductions { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<DeckCard> DeckCard { get; set; }
        public DbSet<Param> Params { get; set; }
        public DbSet<FavVariant> FavCards { get; set; }
        public DbSet<CollectedCard> CollectedCard { get; set; }
        public DbSet<PriceLine> PriceLine { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CardTraduction>().HasKey(m => new { m.CardUuid, m.Language});
            modelBuilder.Entity<DeckCard>().HasKey(m => new { m.DeckId, m.CardUuid});
            modelBuilder.Entity<Tag>().Property(e => e.TagId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Deck>().Property(e => e.DeckId).ValueGeneratedOnAdd();
        }

    }

}
