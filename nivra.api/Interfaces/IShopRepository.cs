namespace backend.Interfaces;

public interface IShopRepository<T> where T : class, IShopEntity
{
    public IEnumerable<T> GetAllItems();
    public void DeleteById(int id);
    public void GetById(int id);
    public void UpdateById(int id);
    public void Add(T value);
    
}