using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MaGeek.Data.Entities;
using MaGeek.Entities;
using Plaziat.CommonWpf;
using Path = System.IO.Path;

namespace MaGeek.Data
{

    public class LocalDb : DbContext
    {

        #region Attributes

        string[] Tables = {
            "CREATE TABLE \"Params\" (\r\n\t\"ParamName\"\tTEXT,\r\n\t\"ParamValue\"\tTEXT\r\n)",
            "CREATE TABLE \"Tags\" (\r\n\t\"Id\"\tINTEGER,\r\n\t\"Tag\"\tTEXT,\r\n\t\"CardId\"\tINTEGER,\r\n\tPRIMARY KEY(\"Id\")\r\n)",
            "CREATE TABLE \"cardVariants\" (\r\n\t\"Id\"\tTEXT,\r\n\t\"ImageUrl\"\tTEXT,\r\n\t\"Rarity\"\tTEXT,\r\n\t\"SetName\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"IsCustom\"\tINTEGER,\r\n\t\"CustomName\"\tTEXT\r\n)",
            "CREATE TABLE \"cards\" (\r\n\t\"CardId\"\tTEXT,\r\n\t\"Type\"\tTEXT,\r\n\t\"ManaCost\"\tTEXT,\r\n\t\"Cmc\"\tINTEGER,\r\n\t\"Text\"\tTEXT,\r\n\t\"Power\"\tTEXT,\r\n\t\"Toughness\"\tTEXT,\r\n\t\"CollectedQuantity\"\tINTEGER,\r\n\t\"FavouriteVariant\"\tTEXT\r\n)",
            "CREATE TABLE \"cardsInDecks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Quantity\"\tINTEGER,\r\n\t\"RelationType\"\tINTEGER,\r\n\tPRIMARY KEY(\"CardId\",\"DeckId\")\r\n)",
            "CREATE TABLE \"decks\" (\r\n\t\"DeckId\"\tINTEGER,\r\n\t\"Title\"\tTEXT,\r\n\tPRIMARY KEY(\"DeckId\")\r\n)",
            "CREATE TABLE \"traductions\" (\r\n\t\"TraductionId\"\tINTEGER,\r\n\t\"CardId\"\tTEXT,\r\n\t\"Language\"\tTEXT,\r\n\t\"TraductedName\"\tTEXT,\r\n\tPRIMARY KEY(\"TraductionId\")\r\n)",
        };

        string DbPath { get { return Path.Combine(App.RoamingFolder, "MaGeek.db"); } }

        string ConnexionString { get { return "Data Source = " + DbPath; } }

        private string BackupName
        {
            get
            {
                string s = "MaGeek " + DateTime.Now.ToString() + ".db";
                s = s.Replace('/', '_').Replace(':', '_');
                return s;

            }
        }

        #endregion

        #region Entity Framework

        public DbSet<MagicCard> cards { get; set; }
        public DbSet<MagicCardVariant> cardVariants { get; set; }
        public DbSet<CardTraduction> traductions { get; set; }
        public DbSet<MagicDeck> decks { get; set; }
        public DbSet<CardDeckRelation> cardsInDecks { get; set; }
        public DbSet<CardTag> Tags { get; set; }
        public DbSet<Param> Params { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnexionString);
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
            if (File.Exists(DbPath)) return;
            SqliteConnection dbCo = new SqliteConnection(ConnexionString);
            dbCo.Open();
            foreach (string instruction in Tables) new SqliteCommand(instruction, dbCo).ExecuteNonQuery();
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
                    using (var localDb = new SqliteConnection(ConnexionString))
                    using (var backupDb = new SqliteConnection("Data Source = " + Path.Combine(saveFolder, BackupName)))
                    {
                        localDb.Open();
                        backupDb.Open();
                        localDb.BackupDatabase(backupDb);
                        localDb.Close();
                        backupDb.Close();
                    }
                    MessageBoxHelper.ShowMsg("DB saved successfully.");
                }
                catch (IOException iox)
                {
                    MessageBoxHelper.ShowMsg("DB save failed : " + iox.Message);
                }
            }
        }

        public void EraseDb()
        {
            if (!MessageBoxHelper.AskUser("Do you really want to erase all card data?")) return;
            EmptyDb();
            MessageBoxHelper.ShowMsg("DB successfully erased.");
            App.Restart();
        }

        private void EmptyDb()
        {
            foreach (var row in from o in cards select o) cards.Remove(row);
            foreach (var row in from o in cardVariants select o) cardVariants.Remove(row);
            foreach (var row in from o in traductions select o) traductions.Remove(row);
            foreach (var row in from o in decks select o) decks.Remove(row);
            foreach (var row in from o in cardsInDecks select o) cardsInDecks.Remove(row);
            foreach (var row in from o in Tags select o) Tags.Remove(row);
            SaveChanges();
        }

        public void RestoreDb() // TODO DEBUG
        {
            string loadFile = SelectFileHelper.SelectAFile("Db Files (.db)|*.db");
            if (loadFile != null)
            {
                try
                {
                    EmptyDb();
                    using (var restoreDb = new SqliteConnection("Data Source = " + loadFile ))
                    {
                        restoreDb.Open();

                        foreach (var row in ReadTable(restoreDb, "cards", 9))
                        {
                            cards.Add(
                                new MagicCard()
                                {
                                    CardId = row[0],
                                    Type = row[1],
                                    ManaCost = row[2],
                                    Cmc = int.Parse(row[3]),
                                    Text = row[4],
                                    Power = row[5],
                                    Toughness = row[6],
                                    CollectedQuantity = int.Parse(row[7]),
                                    FavouriteVariant = row[8],
                                }
                            );
                        }
                        foreach (var row in ReadTable(restoreDb, "cardVariants", 7))
                        {
                            var v = new MagicCardVariant()
                            {
                                Id = row[0],
                                ImageUrl = row[1],
                                Rarity = row[2],
                                SetName = row[3],
                                Card = cards.Where(x => x.CardId == row[4]).FirstOrDefault(),
                                IsCustom = int.Parse(row[5]),
                                CustomName = row[6],
                            };
                            cardVariants.Add(v);
                            if (v.Card.Variants == null) v.Card.Variants = new List<MagicCardVariant>();    
                            v.Card.Variants.Add(v);
                         }
                        foreach (var row in ReadTable(restoreDb, "traductions", 4))
                        {
                            var t = new CardTraduction()
                            {
                                TraductionId = int.Parse(row[0]),
                                CardId = row[1],
                                Card = cards.Where(x => x.CardId == row[1]).FirstOrDefault(),
                                Language = row[2],
                                TraductedName = row[3],
                            };
                            traductions.Add(t);
                            if (t.Card.Traductions == null) t.Card.Traductions = new List<CardTraduction>();
                            t.Card.Traductions.Add(t);
                        }
                        foreach (var row in ReadTable(restoreDb, "decks", 2))
                        {
                            decks.Add(
                                new MagicDeck()
                                {
                                    DeckId = int.Parse(row[0]),
                                    Title = row[1],
                                }
                            );
                        }
                        foreach (var row in ReadTable(restoreDb, "cardsInDecks", 4))
                        {
                            var c = new CardDeckRelation()
                            {
                                DeckId = int.Parse(row[0]),
                                CardId = row[1],
                                Quantity = int.Parse(row[2]),
                                RelationType = int.Parse(row[3]),
                                Deck = decks.Where(x => x.DeckId == int.Parse(row[0])).FirstOrDefault(),
                                Card = cardVariants.Where(x => x.Id == row[1]).FirstOrDefault(),
                            };
                            cardsInDecks.Add(c);
                            if (c.Deck.CardRelations == null) c.Deck.CardRelations = new System.Collections.ObjectModel.ObservableCollection<CardDeckRelation>();
                            c.Deck.CardRelations.Add(c);
                            if (c.Card.DeckRelations == null) c.Card.DeckRelations = new System.Collections.ObjectModel.ObservableCollection<CardDeckRelation>();   
                            c.Card.DeckRelations.Add(c);
                        }
                        foreach (var row in ReadTable(restoreDb, "Tags", 3))
                        {
                            Tags.Add(
                                new CardTag()
                                {
                                    Id = int.Parse(row[0]),
                                    Tag = row[1],
                                    CardId = row[2],
                                    Card = cards.Where(x => x.CardId == row[2]).FirstOrDefault(),
                                }
                            );
                        }
                        restoreDb.Close();
                        SaveChanges();
                    }
                    MessageBoxHelper.ShowMsg("DB successfully loaded");
                    App.Restart();
                }
                catch (IOException iox)
                {
                    MessageBoxHelper.ShowMsg("DB load failed : " + iox.Message);
                }
            }
        }
        static List<string[]> ReadTable(SqliteConnection conn, string tableName, int columnCount)
        {
            SqliteCommand sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM "+tableName;
            List<string[]> retour = new();
            SqliteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string[] row = new string[columnCount];
                for(int i=0;i< columnCount; i++)
                {
                    try { row[i] = sqlite_datareader.GetString(i); } catch { }
                }
                retour.Add(row);
            }
            return retour;
        }

        #endregion

    }

}
