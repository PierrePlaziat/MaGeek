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

        public DbSet<MagicCard> Cards { get; set; }
        public DbSet<MagicCardVariant> CardVariants { get; set; }
        public DbSet<CardTraduction> CardTraductions { get; set; } // TODO get foreign names from another source with more complete data
        public DbSet<Legality> Legalities { get; set; }
        public DbSet<Rule> CardRules { get; set; }
        public DbSet<CardCardRelation> CardRelations { get; set; }

        public DbSet<MagicDeck> Decks { get; set; }
        public DbSet<CardDeckRelation> CardsInDecks { get; set; }
        
        public DbSet<CardTag> Tags { get; set; }
        public DbSet<Param> Params { get; set; }
        
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
                .Property(e => e.LegalityId).ValueGeneratedOnAdd();

            modelBuilder.Entity<CardCardRelation>()
                .Property(e => e.RelationId).ValueGeneratedOnAdd();

            modelBuilder.Entity<Rule>()
                .Property(e => e.RuleId).ValueGeneratedOnAdd();
        }

        internal void DeleteAllContent()
        {
            Cards.ExecuteDeleteAsync();
            CardVariants.ExecuteDeleteAsync();
            Legalities.ExecuteDeleteAsync();
            CardRules.ExecuteDeleteAsync();
            CardRelations.ExecuteDeleteAsync();
            Decks.ExecuteDeleteAsync();
            CardsInDecks.ExecuteDeleteAsync();
            Tags.ExecuteDeleteAsync();
            App.Restart();
        }

    }

}
