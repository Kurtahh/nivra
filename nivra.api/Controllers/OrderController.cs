using backend.Interfaces;
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase, IOrderController
{
    private readonly IOrderService _service;

    public OrderController(IOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult GetOrders()
    {
        return Ok(_service.GetOrders());
    }

    [HttpGet("{id:int}")]
    public IActionResult GetOrderById(int id)
    {
        var order = _service.GetOrderById(id);
        if (order == null)
        {
            return NotFound(new { message = "Order not found." });
        }

        return Ok(order);
    }

    [HttpPost]
    public IActionResult CreateOrder([FromBody] Order order)
    {
        try
        {
            var createdOrder = _service.CreateOrder(order);
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, createdOrder);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public IActionResult UpdateOrder(int id, [FromBody] Order order)
    {
        try
        {
            var updatedOrder = _service.UpdateOrder(id, order);
            if (updatedOrder == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            return Ok(updatedOrder);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteOrder(int id)
    {
        if (!_service.DeleteOrder(id))
        {
            return NotFound(new { message = "Order not found." });
        }

        return NoContent();
    }
}
