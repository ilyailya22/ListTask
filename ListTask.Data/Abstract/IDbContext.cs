using Microsoft.EntityFrameworkCore;

namespace ListTask.Data.Abstract;

public interface IDbContext : IDisposable
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    Task SaveChangesAsync();
}