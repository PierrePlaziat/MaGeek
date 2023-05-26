using Microsoft.EntityFrameworkCore;
using MaGeek.Entities;

namespace MaGeek.Framework.Data
{

    /// <summary>
    /// Entity Framework over Sqlite
    /// representing a MTG card collection
    /// </summary>
    public class MageekDbContext : DbContext
    {

        SqliteDbInfos dbData;

        public MageekDbContext(SqliteDbInfos dbData) { this.dbData = dbData; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(dbData.ConnexionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<CardLegality> CardLegalities { get; set; }
        public DbSet<CardModel> CardModels { get; set; }
        public DbSet<CardRelation> CardRelations { get; set; }
        public DbSet<CardRule> CardRules { get; set; }
        public DbSet<CardTag> CardTags { get; set; }
        public DbSet<CardTraduction> CardTraductions { get; set; }
        public DbSet<CardVariant> CardVariants { get; set; }
        public DbSet<Deck> Decks { get; set; }
        public DbSet<DeckCard> DeckCards { get; set; }
        public DbSet<Param> Params { get; set; }
        public DbSet<Set> Sets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeckCard>()
                        .HasKey(q => new { q.DeckId, q.CardId });

            modelBuilder.Entity<CardVariant>()
                        .HasOne(e => e.Card).WithMany(e => e.Variants);

            //modelBuilder.Entity<CardVariant>()
            //            .HasOne(e => e.Set).WithMany(e => e.SetCards);

            modelBuilder.Entity<CardTraduction>()
                        .HasOne(e => e.Card).WithMany(e => e.Traductions);

            modelBuilder.Entity<DeckCard>()
                        .HasOne(s => s.Deck).WithMany(e => e.DeckCards)
                        .HasForeignKey(t => t.DeckId);

            modelBuilder.Entity<DeckCard>()
                        .HasOne(s => s.Card).WithMany(e => e.DeckRelations)
                        .HasForeignKey(t => t.CardId);

            modelBuilder.Entity<Deck>()
                        .Property(e => e.DeckId).ValueGeneratedOnAdd();

            modelBuilder.Entity<CardTraduction>()
                        .Property(e => e.TraductionId).ValueGeneratedOnAdd();

            modelBuilder.Entity<CardTag>()
                        .Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<CardLegality>()
                        .Property(e => e.LegalityId).ValueGeneratedOnAdd();

            modelBuilder.Entity<CardRelation>()
                        .Property(e => e.RelationId).ValueGeneratedOnAdd();

            modelBuilder.Entity<CardRule>()
                        .Property(e => e.RuleId).ValueGeneratedOnAdd();
        }

        // TODO more granullar DB gestion
        internal void DeleteAllContent()
        {
            CardModels.ExecuteDeleteAsync();
            CardVariants.ExecuteDeleteAsync();
            CardLegalities.ExecuteDeleteAsync();
            CardRules.ExecuteDeleteAsync();
            CardRelations.ExecuteDeleteAsync();
            Decks.ExecuteDeleteAsync();
            DeckCards.ExecuteDeleteAsync();
            CardTags.ExecuteDeleteAsync();
            App.Restart();
        }

    }

}
