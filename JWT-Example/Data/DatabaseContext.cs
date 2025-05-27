using JWT_Example.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWT_Example.Data;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>().HasData(new UserRole
            {
                Id = 1,
                Name = "User"
            }, new UserRole
            {
                Id = 2,
                Name = "Admin"
            }
        );
    }
}