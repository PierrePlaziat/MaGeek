using MaGeek.Data.Entities;
using MaGeek.Entities;
using Microsoft.EntityFrameworkCore;

namespace MaGeek.Data
{
    public class SqliteContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Mtg.db");
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<MagicCard> cards { get; set; }
        public DbSet<MagicCardVariant> cardVariants { get; set; }
        public DbSet<CardTraduction> traductions { get; set; }
        public DbSet<MagicDeck> decks { get; set; }
        public DbSet<CardDeckRelation> cardsInDecks { get; set; }
        public DbSet<Param> Params { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CardDeckRelation>().HasKey(q =>
               new {
                   q.DeckId,
                   q.CardId,
               });

            modelBuilder.Entity<MagicCardVariant>()
                        .HasOne(e => e.Card)
                        .WithMany(e => e.Variants);

            modelBuilder.Entity<CardTraduction>()
                        .HasOne(e => e.Card)
                        .WithMany(e => e.Traductions);

            modelBuilder.Entity<CardDeckRelation>()
                        .HasOne(s => s.Deck)
                        .WithMany(e => e.CardRelations)
                        .HasForeignKey(t => t.DeckId);

            modelBuilder.Entity<CardDeckRelation>()
                        .HasOne(s => s.Card)
                        .WithMany(e => e.DeckRelations)
                        .HasForeignKey(t => t.CardId);


            modelBuilder.Entity<MagicDeck>().Property(e => e.DeckId).ValueGeneratedOnAdd();
            modelBuilder.Entity<CardTraduction>().Property(e => e.TraductionId).ValueGeneratedOnAdd();
        }

    }
}
