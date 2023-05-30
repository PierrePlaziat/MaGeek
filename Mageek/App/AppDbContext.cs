using Microsoft.EntityFrameworkCore;
using MaGeek.Entities;
using MaGeek.Framework.Data;

namespace MaGeek
{

    /// <summary>
    /// Entity Framework over Sqlite
    /// representing a MTG card collection
    /// </summary>
    public class AppDbContext : DbContext
    {

        #region CTOR

        SqliteDbInfos dbData;

        public AppDbContext(SqliteDbInfos dbData) { this.dbData = dbData; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(dbData.ConnexionString);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        #endregion

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
        
        //public DbSet<Set> Sets { get; set; }

        public DbSet<FavVariant> FavCards { get; set; }
        public DbSet<User_GotCard> User_GotCards { get; set; }

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

        // This has to match DbSets
        public readonly static string[] TablesCreationString = new string[] {
            "CREATE TABLE \"CardLegalities\" (\r\n\t\"LegalityId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"Format\"\tTEXT,\r\n\t\"IsLegal\"\tTEXT,\r\n\tPRIMARY KEY(\"LegalityId\")\r\n);",
            "CREATE TABLE \"CardModels\" (\r\n\t\"CardId\"\tTEXT,\r\n\t\"Type\"\tTEXT,\r\n\t\"ManaCost\"\tREAL,\r\n\t\"Cmc\"\tNUMERIC,\r\n\t\"ColorIdentity\"\tTEXT,\r\n\t\"DevotionB\"\tINTEGER,\r\n\t\"DevotionW\"\tINTEGER,\r\n\t\"DevotionU\"\tINTEGER,\r\n\t\"DevotionR\"\tINTEGER,\r\n\t\"DevotionG\"\tINTEGER,\r\n\t\"Text\"\tTEXT,\r\n\t\"Keywords\"\tTEXT,\r\n\t\"Power\"\tTEXT,\r\n\t\"Toughness\"\tTEXT,\r\n\t\"FavouriteVariant\"\tTEXT,\r\n\t\"Got\"\tINTEGER\r\n);",
            "CREATE TABLE \"CardRelations\" (\r\n\t\"RelationId\"\tINTEGER,\r\n\t\"Card1Id\"\tTEXT,\r\n\t\"Card2Id\"\tTEXT,\r\n\t\"LastUpdate\"\tTEXT,\r\n\t\"RelationType\"\tTEXT,\r\n\tPRIMARY KEY(\"RelationId\")\r\n);",
            "CREATE TABLE \"CardRules\" (\r\n\t\"RuleId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Source\"\tTEXT,\r\n\t\"PublicationDate\"\tTEXT,\r\n\t\"Comment\"\tTEXT,\r\n\t\"ObjectType\"\tTEXT,\r\n\tPRIMARY KEY(\"RuleId\")\r\n);",
            "CREATE TABLE \"CardTags\" (\r\n\t\"Id\"\tINTEGER,\r\n\t\"Tag\"\tTEXT,\r\n\t\"CardId\"\tINTEGER,\r\n\tPRIMARY KEY(\"Id\")\r\n);",
            "CREATE TABLE \"CardTraductions\" (\r\n\t\"TraductionId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"TraductedName\"\tTEXT,\r\n\t\"Normalized\"\tTEXT,\r\n\tPRIMARY KEY(\"TraductionId\")\r\n);",
            "CREATE TABLE \"CardVariants\" (\r\n\t\"Id\"\tTEXT,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Rarity\"\tTEXT,\r\n\t\"SetName\"\tTEXT,\r\n\t\"ImageUrl_Front\"\tTEXT,\r\n\t\"ImageUrl_Back\"\tTEXT,\r\n\t\"ValueEur\"\tTEXT,\r\n\t\"ValueUsd\"\tTEXT,\r\n\t\"EdhRecRank\"\tINTEGER,\r\n\t\"IsCustom\"\tINTEGER,\r\n\t\"CustomName\"\tTEXT,\r\n\t\"Got\"\tINTEGER,\r\n\t\"LastUpdate\"\tTEXT\r\n);",
            "CREATE TABLE \"DeckCards\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER,\r\n\tPRIMARY KEY(\"CardId\",\"DeckId\")\r\n);",
            "CREATE TABLE \"Decks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"Title\"\tTEXT,\r\n\t\"Description\"\tTEXT,\r\n\t\"DeckColors\"\tTEXT,\r\n\t\"CardCount\"\tINTEGER,\r\n\tPRIMARY KEY(\"DeckId\")\r\n);",
            "CREATE TABLE \"Params\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n);",
            "CREATE TABLE \"Sets\" (\r\n\t\"Name\"\tTEXT,\r\n\t\"Type\"\tTEXT,\r\n\t\"Svg\"\tTEXT,\r\n\t\"Date\"\tTEXT,\r\n\tPRIMARY KEY(\"Name\")\r\n);",
            "CREATE TABLE \"User_FavCards\" (\r\n\t\"CardModelId\"\tTEXT,\r\n\t\"CardVariantId\"\tTEXT,\r\n\tPRIMARY KEY(\"CardModelId\")\r\n);",
            "CREATE TABLE \"User_GotCards\" (\r\n\t\"CardVariantId\"\tTEXT,\r\n\t\"CardModelId\"\tTEXT,\r\n\t\"Got\"\tINTEGER,\r\n\tPRIMARY KEY(\"CardVariantId\")\r\n);",
        };

    }

}
