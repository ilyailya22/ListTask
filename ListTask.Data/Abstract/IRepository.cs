namespace ListTask.Data.Abstract;

public interface IRepository<T> where T : class
{
    void Add(T entity);
    
    void Update(T entity);
    
    void Delete(T entity);
    
    IQueryable<T> GetAll();
}