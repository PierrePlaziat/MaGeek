using Microsoft.EntityFrameworkCore;
using MaGeek.AppData.Entities;
using Mageek.AppData;

namespace MaGeek.AppBusiness
{

    /// <summary>
    /// Entity Framework over Sqlite
    /// representing a MTG card collection
    /// </summary>
    public class MageekDbContext : DbContext
    {

        SqliteDbInfos dbData;

        public MageekDbContext(SqliteDbInfos dbData)
        {
            this.dbData = dbData;
        }

        public DbSet<MagicCard> cards { get; set; }
        public DbSet<MagicCardVariant> cardVariants { get; set; }
        public DbSet<CardTraduction> traductions { get; set; }
        public DbSet<MagicDeck> decks { get; set; }
        public DbSet<CardDeckRelation> cardsInDecks { get; set; }
        public DbSet<CardTag> Tags { get; set; }
        public DbSet<Param> Params { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Legality> Legalities { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(dbData.ConnexionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CardDeckRelation>()
                .HasKey(q => new { q.DeckId, q.CardId });

            modelBuilder.Entity<MagicCardVariant>()
                .HasOne(e => e.Card).WithMany(e => e.Variants);

            modelBuilder.Entity<CardTraduction>()
                .HasOne(e => e.Card).WithMany(e => e.Traductions);

            modelBuilder.Entity<CardDeckRelation>()
                .HasOne(s => s.Deck).WithMany(e => e.CardRelations)
                .HasForeignKey(t => t.DeckId);

            modelBuilder.Entity<CardDeckRelation>()
                .HasOne(s => s.Card).WithMany(e => e.DeckRelations)
                .HasForeignKey(t => t.CardId);

            modelBuilder.Entity<MagicDeck>()
                .Property(e => e.DeckId).ValueGeneratedOnAdd();

            modelBuilder.Entity<CardTraduction>()
                .Property(e => e.TraductionId).ValueGeneratedOnAdd();

            modelBuilder.Entity<CardTag>()
                .Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Legality>()
                .Property(e => e.Id).ValueGeneratedOnAdd();
        }

        internal void DeleteAllContent()
        {
            cards.ExecuteDeleteAsync();
            cardVariants.ExecuteDeleteAsync();
            traductions.ExecuteDeleteAsync();
            decks.ExecuteDeleteAsync();
            cardsInDecks.ExecuteDeleteAsync();
            Tags.ExecuteDeleteAsync();
            App.Restart();
        }

    }

}
