using Microsoft.EntityFrameworkCore;
using TodoAPIAssignment.DataAccessLibrary.Models;

namespace TodoAPIAssignment.DataAccessLibrary;

public class DataDbContext : DbContext
{
    public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
    {
        
    }

    public DbSet<Todo> Todos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Todo>().HasKey("Id");
        modelBuilder.Entity<Todo>().ToContainer("TodosAssignment_Todos").HasPartitionKey(todo => todo.Id);

        base.OnModelCreating(modelBuilder);
    }
}
