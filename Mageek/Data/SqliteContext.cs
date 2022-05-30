using MaGeek.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MaGeek.Data
{
    public class SqliteContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=D:\\GitPers\\MaGeek\\Mtg.db");
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<MagicCardVariant> cardVariants { get; set; }
        public DbSet<MagicCard> cards { get; set; }
        public DbSet<MagicDeck> decks { get; set; }
        public DbSet<CardTraduction> traductions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MagicCardVariant>()
                        .HasOne(e => e.card)
                        .WithMany(e => e.variants);
            modelBuilder.Entity<MagicDeck>()
                        .HasMany<MagicCard>(s => s.Cards)
                        .WithMany(c => c.Decks);
            modelBuilder.Entity<MagicDeck>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<CardTraduction>().Property(e => e.Id).ValueGeneratedOnAdd();
        }

    }
}
