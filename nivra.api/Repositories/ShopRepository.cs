using backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public class ShopRepository<T> : IShopRepository<T> where T : class, IShopEntity
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _table;
        
    public ShopRepository(AppDbContext context)
    {
        _context = context;
        _table = _context.Set<T>();
    }
    
    public IEnumerable<T> GetAllItems()
    {
        return [.. _table];
    }

    public void Delete(T entity)
    {
        _table.Remove(entity);
        _context.SaveChanges();
    }

    public T? GetById(int id)
    
    {
        return _table.Find(id);
    }

    public void Update(T obj)
    {
        _table.Attach(obj);
        _context.Entry(obj).State = EntityState.Modified;
        _context.SaveChanges();
    }

    public void Add(T value)
    {
        _table.Add(value);
        _context.SaveChanges();
    }
}