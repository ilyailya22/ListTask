using ListTask.Data.Abstract;

namespace ListTask.Data.Concrete;

public class UnitOfWork(IDbContext dbContext) : IUnitOfWork
{
    public async Task CommitAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}