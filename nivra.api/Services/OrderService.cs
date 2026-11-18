using backend.Interfaces;
using backend.Models;

namespace backend.Services;

public class OrderService : IOrderService
{
    private readonly IShopRepository<Order> _repository;

    public OrderService(IShopRepository<Order> repository)
    {
        _repository = repository;
    }

    public IEnumerable<Order> GetOrders()
    {
        return _repository.GetAllItems();
    }

    public Order? GetOrderById(int id)
    {
        return _repository.GetById(id);
    }

    public Order CreateOrder(Order order)
    {
        ValidateOrder(order);

        order.Id = 0;
        _repository.Add(order);

        return order;
    }

    public Order? UpdateOrder(int id, Order order)
    {
        ValidateOrder(order);

        var existingOrder = _repository.GetById(id);
        if (existingOrder == null)
        {
            return null;
        }

        existingOrder.Name = order.Name;
        existingOrder.Price = order.Price;

        _repository.Update(existingOrder);

        return existingOrder;
    }

    public bool DeleteOrder(int id)
    {
        var existingOrder = _repository.GetById(id);
        if (existingOrder == null)
        {
            return false;
        }

        _repository.Delete(existingOrder);

        return true;
    }

    private static void ValidateOrder(Order order)
    {
        if (string.IsNullOrWhiteSpace(order.Name))
        {
            throw new ArgumentException("Order name cannot be empty.");
        }

        if (order.Price < 0)
        {
            throw new ArgumentException("Order price cannot be negative.");
        }
    }
}
