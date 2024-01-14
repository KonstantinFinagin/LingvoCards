using Microsoft.EntityFrameworkCore;

namespace LingvoCards.Dal.Repositories;

public class BaseRepository<T>(DbContext context) : IRepository<T>
    where T : class
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public IEnumerable<T> GetAll()
    {
        return DbSet.ToList();
    }

    public T? GetById(Guid id)
    {
        return DbSet.Find(id);
    }

    public void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public void Update(T entity)
    {
        context.Update(entity);
    }

    public void Remove(T entity)
    {
        if (context.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }
        DbSet.Remove(entity);
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}