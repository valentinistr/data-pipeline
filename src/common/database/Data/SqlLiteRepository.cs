using Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Database.Data;

internal class SqlLiteRepository<T>(DbSet<T> dbSet) : IRepository<T> where T : class
{
    public IQueryable<T> Query => dbSet.AsQueryable();

    public void Add(T entity) => dbSet.Add(entity);

    public void Remove(T entity) => dbSet.Remove(entity);
}
