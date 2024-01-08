namespace LingvoCards.Dal;

public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T? GetById(Guid id);
    void Add(T entity);
    void Remove(T entity);
    void SaveChanges();
}