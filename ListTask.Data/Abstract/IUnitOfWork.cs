namespace ListTask.Data.Abstract;

public interface IUnitOfWork
{
    Task CommitAsync();
}