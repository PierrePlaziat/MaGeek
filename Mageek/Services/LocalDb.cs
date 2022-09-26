using System;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Plaziat.CommonWpf;
using MaGeek.Data.Entities;
using MaGeek.Entities;
using System.Linq;

namespace MaGeek.Data
{
    public class LocalDb : DbContext
    {

        public DbSet<MagicCard> cards { get; set; }
        public DbSet<MagicCardVariant> cardVariants { get; set; }
        public DbSet<CardTraduction> traductions { get; set; }
        public DbSet<MagicDeck> decks { get; set; }
        public DbSet<CardDeckRelation> cardsInDecks { get; set; }
        public DbSet<CardTag> Tags { get; set; }
        public DbSet<Param> Params { get; set; }

        #region Attributes

        string dbName = "MaGeek.db";
        string[] createTables = {
            "CREATE TABLE \"Params\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n)",
            "CREATE TABLE \"Tags\" (\r\n\t\"Id\"\tINTEGER,\r\n\t\"Tag\"\tTEXT,\r\n\t\"CardId\"\tINTEGER,\r\n\tPRIMARY KEY(\"Id\")\r\n)",
            "CREATE TABLE \"cardVariants\" (\r\n\t\"Id\"\tTEXT,\r\n\t\"ImageUrl\"\tTEXT,\r\n\t\"Rarity\"\tTEXT,\r\n\t\"SetName\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"IsCustom\"\tINTEGER,\r\n\t\"CustomName\"\tTEXT\r\n)",
            "CREATE TABLE \"cards\" (\r\n\t\"CardId\"\tTEXT,\r\n\t\"Type\"\tTEXT,\r\n\t\"ManaCost\"\tTEXT,\r\n\t\"Cmc\"\tINTEGER,\r\n\t\"Text\"\tTEXT,\r\n\t\"Power\"\tTEXT,\r\n\t\"Toughness\"\tTEXT,\r\n\t\"CollectedQuantity\"\tINTEGER,\r\n\t\"FavouriteVariant\"\tTEXT\r\n)",
            "CREATE TABLE \"cardsInDecks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER,\r\n\tPRIMARY KEY(\"CardId\",\"DeckId\")\r\n)",
            "CREATE TABLE \"decks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"Title\"\tTEXT,\r\n\tPRIMARY KEY(\"DeckId\")\r\n)",
            "CREATE TABLE \"traductions\" (\r\n\t\"TraductionId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"TraductedName\"\tTEXT,\r\n\tPRIMARY KEY(\"TraductionId\")\r\n)",
        };

        string dbPath { get { return System.Environment.SpecialFolder.LocalApplicationData + dbName; } }
        string connexionString { get { return "Data Source = " + dbPath; } }

        #endregion

        #region Entity Framework

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connexionString);
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.EnableSensitiveDataLogging();
        }

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
            modelBuilder.Entity<CardTag>().Property(e => e.Id).ValueGeneratedOnAdd();
        }

        #endregion

        #region Sqlite Management

        public void InitDb()
        {
            if (File.Exists(dbPath)) return;
            SqliteConnection dbCo = new SqliteConnection(connexionString);
            dbCo.Open();
            foreach (string instruction in createTables)
                new SqliteCommand(instruction, dbCo).ExecuteNonQuery();
            SaveChanges();
            dbCo.Close();
        }

        public void BackupDb()
        {
            string saveFolder = SelectFileHelper.SelectAFolder();
            if (saveFolder != null)
            {
                try
                {
                    File.Copy(dbPath, saveFolder + "\\MaGeepBackup_" + DateTime.Now + ".db", true);
                    MessageBoxHelper.ShowMsg("DB successfully saved");
                }
                catch (IOException iox)
                {
                    MessageBoxHelper.ShowMsg("DB save failed : " + iox.Message);
                }
            }
        }

        public void RestoreDb()
        {
            string loadFile = SelectFileHelper.SelectAFile("MageekDb Files (*.MageekDb)", ".MageekDb");
            if (loadFile != null)
            {
                try
                {
                    File.Copy(loadFile, dbPath, true);
                    MessageBoxHelper.ShowMsg("DB successfully loaded");
                    App.Restart();
                }
                catch (IOException iox)
                {
                    MessageBoxHelper.ShowMsg("DB load failed : " + iox.Message);
                }
            }
        }
        
        public void EraseDb()
        {
            if (!MessageBoxHelper.AskUser("Are you sure?")) return;

            var cardRows = from o in cards select o;
            foreach (var row in cardRows) cards.Remove(row);

            var cardVariantsrows = from o in cardVariants select o;
            foreach (var row in cardVariantsrows) cardVariants.Remove(row);

            var traductionsrows = from o in traductions select o;
            foreach (var row in traductionsrows) traductions.Remove(row);

            var decksrows = from o in decks select o;
            foreach (var row in decksrows) decks.Remove(row);

            var cardsInDecksrows = from o in cardsInDecks select o;
            foreach (var row in cardsInDecksrows) cardsInDecks.Remove(row);

            var Tagsrows = from o in Tags select o;
            foreach (var row in Tagsrows) Tags.Remove(row);

            SaveChanges();

            MessageBoxHelper.ShowMsg("DB successfully erased");

            System.Diagnostics.Process.Start(App.ResourceAssembly.Location);
            App.Current.Shutdown();

        }

        #endregion

    }

}
