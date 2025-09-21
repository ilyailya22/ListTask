using ListTask.Data.Abstract;
using Microsoft.EntityFrameworkCore;

namespace ListTask.Data.Concrete;

public class Repository<T>(IDbContext dbContext) : IRepository<T>
    where T : class
{
    public void Add(T entity)
    {
        dbContext.Set<T>().Add(entity);
    }

    public void Update(T entity)
    {
        dbContext.Set<T>().Update(entity);
    }

    public void Delete(T entity)
    {
        dbContext.Set<T>().Remove(entity);
    }

    public IQueryable<T> GetAll()
    {
        return dbContext.Set<T>().AsNoTrackingWithIdentityResolution();
    }
}