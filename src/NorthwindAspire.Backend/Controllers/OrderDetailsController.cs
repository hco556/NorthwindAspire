using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;



public class OrderDetailsController : ODataController
{
    private readonly NorthwindContext _context;

    public OrderDetailsController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all order details with OData query support
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<OrderDetail> Get()
    {
        return _context.OrderDetails;
    }

    /// <summary>
    /// Get order detail by OrderId and ProductId
    /// </summary>
    [HttpGet("{key}")]
    [EnableQuery]
    public async Task<ActionResult<OrderDetail>> GetById([FromRoute] int key)
    {
        var orderDetail = await _context.OrderDetails
            .FirstOrDefaultAsync(od => od.OrderId == key);
        if (orderDetail == null)
        {
            return NotFound();
        }
        return Ok(orderDetail);
    }

    /// <summary>
    /// Create a new order detail
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<OrderDetail>> Post([FromBody] OrderDetail orderDetail)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.OrderDetails.Add(orderDetail);
        await _context.SaveChangesAsync();
        return Created($"odata/orderdetails({orderDetail.OrderId},{orderDetail.ProductId})", orderDetail);
    }

    /// <summary>
    /// Update an order detail
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(int key, [FromBody] Delta<OrderDetail> delta)
    {
        var orderDetail = await _context.OrderDetails
            .FirstOrDefaultAsync(od => od.OrderId == key);
        if (orderDetail == null)
        {
            return NotFound();
        }

        delta.Patch(orderDetail);
        await _context.SaveChangesAsync();
        return Updated(orderDetail);
    }

    /// <summary>
    /// Delete an order detail
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(int key)
    {
        var orderDetail = await _context.OrderDetails
            .FirstOrDefaultAsync(od => od.OrderId == key);
        if (orderDetail == null)
        {
            return NotFound();
        }

        _context.OrderDetails.Remove(orderDetail);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
