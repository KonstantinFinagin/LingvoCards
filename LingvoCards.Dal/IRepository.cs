namespace LingvoCards.Dal;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    void Add(T entity);
    void Remove(T entity);
    Task SaveChangesAsync();
}