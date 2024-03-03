using Microsoft.EntityFrameworkCore;

namespace LingvoCards.Dal.Repositories;

public class BaseRepository<T>(DbContext context) : IRepository<T>
    where T : class
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FindAsync(id);
    }

    public void Add(T entity)
    {
        DbSet.Add(entity);
    }

    public void Update(T entity)
    {
        if (context.Entry(entity).State == EntityState.Detached)
        {
            DbSet.Attach(entity);
        }
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

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
    }
}