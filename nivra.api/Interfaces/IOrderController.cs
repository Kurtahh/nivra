using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Interfaces;

public interface IOrderController
{
    IActionResult GetOrders();
    IActionResult GetOrderById(int id);
    IActionResult CreateOrder(Order order);
    IActionResult UpdateOrder(int id, Order order);
    IActionResult DeleteOrder(int id);
}
