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
        public DbSet<Tag> CardTags { get; set; }
        public DbSet<CardTraduction> CardTraductions { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<DeckCard> DeckCards { get; set; }
        public DbSet<Param> Params { get; set; }
        public DbSet<FavVariant> FavCards { get; set; }
        public DbSet<CollectedCard> CollectedCards { get; set; }
        public DbSet<PriceLine> Prices { get; set; }

    }

}
