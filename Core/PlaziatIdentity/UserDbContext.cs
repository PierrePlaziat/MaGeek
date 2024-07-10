using Microsoft.EntityFrameworkCore;

namespace PlaziatIdentity
{
    public class UserDbContext : DbContext
    {

        readonly string dbPath;

        public UserDbContext(string dbPath) { this.dbPath = dbPath; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = " + dbPath);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().Property(e => e.Id).ValueGeneratedOnAdd();
        }

    }
}
