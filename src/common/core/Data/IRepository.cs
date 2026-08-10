namespace Core.Data;

public interface IRepository<T> where T : class
{
    IQueryable<T> Query { get; }
    void Add(T entity);
    void Remove(T entity);
}
