namespace backend.Interfaces;

public interface IShopRepository<T> where T : class, IShopEntity
{
    public IEnumerable<T> GetAllItems();
    public void Delete(T obj);
    public T? GetById(int id);
    public void Update(T obj);
    public void Add(T value);
    
}