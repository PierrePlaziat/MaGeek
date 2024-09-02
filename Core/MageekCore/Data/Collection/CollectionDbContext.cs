using Microsoft.EntityFrameworkCore;
using MageekCore.Data.Collection.Entities;

namespace MageekCore.Data.Collection
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

        public DbSet<Tag> Tag { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<DeckCard> DeckCard { get; set; }
        public DbSet<Param> Params { get; set; }
        public DbSet<FavVariant> FavVariant { get; set; }
        public DbSet<CollectedCard> CollectedCard { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeckCard>().HasKey(m => new { m.DeckId, m.CardUuid });
            modelBuilder.Entity<Tag>().Property(e => e.TagId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Deck>().Property(e => e.DeckId).ValueGeneratedOnAdd();
        }

    }

}
