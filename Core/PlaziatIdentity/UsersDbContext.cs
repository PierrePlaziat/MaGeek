using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;    
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser {}

public class UsersDbContext : IdentityDbContext<ApplicationUser>
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) {}
}

public class DesignTimeUserDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();
        var builder = new DbContextOptionsBuilder<UsersDbContext>();
        builder.UseSqlite(string.Concat("Data Source = ", PlaziatTools.Paths.File_UserDb));
        return new UsersDbContext(builder.Options);
    }
}
