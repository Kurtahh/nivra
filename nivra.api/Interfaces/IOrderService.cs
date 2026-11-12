using backend.Models;

namespace backend.Interfaces;

public interface IOrderService
{
    IEnumerable<Order> GetOrders();
    Order? GetOrderById(int id);
    Order CreateOrder(Order order);
    Order? UpdateOrder(int id, Order order);
    bool DeleteOrder(int id);
}
