using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Database.Data;

public class SqlLiteRepository<T>(DbSet<T> dbSet) : IRepository<T> where T : class
{
    public IQueryable<T> Query => dbSet.AsQueryable();

    public void Add(T entity) => dbSet.Add(entity);

    public void Remove(T entity) => dbSet.Remove(entity);
}
