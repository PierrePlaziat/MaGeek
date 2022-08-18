using MaGeek.Data.Entities;
using MaGeek.Entities;
using Microsoft.EntityFrameworkCore;
using Plaziat.CommonWpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MaGeek.Data
{
    public class LocalDb : DbContext
    {

        #region Entity Framework

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
        public DbSet<CardTag> Tags { get; set; }


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

        #region Database Management

        public void EraseDb()
        {
            if (!MessageBoxHelper.AskUser("Are you sure?")) return;

            var cardRows = from o in App.Database.cards select o;
            foreach (var row in cardRows) App.Database.cards.Remove(row);

            var cardVariantsrows = from o in App.Database.cardVariants select o;
            foreach (var row in cardVariantsrows) App.Database.cardVariants.Remove(row);

            var traductionsrows = from o in App.Database.traductions select o;
            foreach (var row in traductionsrows) App.Database.traductions.Remove(row);

            var decksrows = from o in App.Database.decks select o;
            foreach (var row in decksrows) App.Database.decks.Remove(row);

            var cardsInDecksrows = from o in App.Database.cardsInDecks select o;
            foreach (var row in cardsInDecksrows) App.Database.cardsInDecks.Remove(row);

            var Tagsrows = from o in App.Database.Tags select o;
            foreach (var row in Tagsrows) App.Database.Tags.Remove(row);

            App.Database.SaveChanges();

            MessageBoxHelper.ShowMsg("DB successfully erased");

            System.Diagnostics.Process.Start(App.ResourceAssembly.Location);
            App.Current.Shutdown();

        }

        public void SaveDb()
        {
            string sourceFile = System.AppDomain.CurrentDomain.BaseDirectory + "Mtg.db";
            Console.WriteLine(sourceFile);
            try
            {
                File.Copy(sourceFile, Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Mtg.db", true);
                MessageBoxHelper.ShowMsg("DB successfully saved");
            }
            catch (IOException iox)
            {
                MessageBoxHelper.ShowMsg(iox.Message);
            }
        }

        public void LoadDb()
        {
            string sourceFile = System.AppDomain.CurrentDomain.BaseDirectory + "Mtg.db";
            Console.WriteLine(sourceFile);
            try
            {
                File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Mtg.db", sourceFile, true);
                MessageBoxHelper.ShowMsg("DB successfully loaded");
            }
            catch (IOException iox)
            {
                MessageBoxHelper.ShowMsg(iox.Message);
            }

            System.Diagnostics.Process.Start(App.ResourceAssembly.Location);
            App.Current.Shutdown();
        }

        public void CleanDb()
        {
            //TODO
            throw new NotImplementedException();
        }

        internal List<string> AvailableTags()
        {
            List<string> tags = new List<string>();
            tags.Add("");
            var x = App.Database.Tags.GroupBy(test => test.Tag)
                .Select(grp => grp.First()).ToList();
            foreach (var v in x)
            {
                tags.Add(v.Tag);
            }
            return tags;
        }

        #endregion

    }

}
