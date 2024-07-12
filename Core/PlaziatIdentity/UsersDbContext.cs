using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;    
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class UsersDbContext : IdentityDbContext<ApplicationUser>
{

    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
    {
    }

}

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{

    public UsersDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();

        var builder = new DbContextOptionsBuilder<UsersDbContext>();

        builder.UseSqlite(
            "Data Source = " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\PlaziatIdentity\\Users.db"
        );

        return new UsersDbContext(builder.Options);
    }

}
