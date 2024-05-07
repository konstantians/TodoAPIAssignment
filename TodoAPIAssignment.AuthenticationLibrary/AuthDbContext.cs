using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.AuthenticationLibrary.Models;

namespace TodoAPIAssignment.AuthenticationLibrary;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
        
    }

    public DbSet<AppUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>().HasKey("Id");
        modelBuilder.Entity<AppUser>().ToContainer("TodosAssignment_Users").HasPartitionKey(user => user.Id);

        base.OnModelCreating(modelBuilder);
    }
}
