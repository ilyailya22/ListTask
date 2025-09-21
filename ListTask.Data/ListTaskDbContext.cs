using ListTask.Data.Abstract;
using ListTask.Data.Mapping;
using Microsoft.EntityFrameworkCore;

namespace ListTask.Data;

public class ListTaskDbContext : DbContext, IDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserMapper());
        modelBuilder.ApplyConfiguration(new TaskListMapper());
        modelBuilder.ApplyConfiguration(new TaskListShareMapper());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=listTask;Username=username;Password=password");
    }

    public Task SaveChangesAsync() => base.SaveChangesAsync();
}