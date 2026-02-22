using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;



public class OrdersController : ODataController
{
    private readonly NorthwindContext _context;

    public OrdersController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all orders with OData query support
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Order> Get()
    {
        return _context.Orders;
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{key}")]
    [EnableQuery]
    public async Task<ActionResult<Order>> GetById(int key)
    {
        var order = await _context.Orders.FindAsync(key);
        if (order == null)
        {
            return NotFound();
        }
        return Ok(order);
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Order>> Post([FromBody] Order order)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return Created($"odata/orders({order.OrderId})", order);
    }

    /// <summary>
    /// Update an order
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(int key, [FromBody] Delta<Order> delta)
    {
        var order = await _context.Orders.FindAsync(key);
        if (order == null)
        {
            return NotFound();
        }

        delta.Patch(order);
        await _context.SaveChangesAsync();
        return Updated(order);
    }

    /// <summary>
    /// Delete an order
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(int key)
    {
        var order = await _context.Orders.FindAsync(key);
        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
