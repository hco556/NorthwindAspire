using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;



public class ShippersController : ODataController
{
    private readonly NorthwindContext _context;

    public ShippersController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all shippers with OData query support
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Shipper> Get()
    {
        return _context.Shippers;
    }

    /// <summary>
    /// Get shipper by ID
    /// </summary>
    [HttpGet("{key}")]
    [EnableQuery]
    public async Task<ActionResult<Shipper>> GetById(int key)
    {
        var shipper = await _context.Shippers.FindAsync(key);
        if (shipper == null)
        {
            return NotFound();
        }
        return Ok(shipper);
    }

    /// <summary>
    /// Create a new shipper
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Shipper>> Post([FromBody] Shipper shipper)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Shippers.Add(shipper);
        await _context.SaveChangesAsync();
        return Created($"odata/shippers({shipper.ShipperId})", shipper);
    }

    /// <summary>
    /// Update a shipper
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(int key, [FromBody] Delta<Shipper> delta)
    {
        var shipper = await _context.Shippers.FindAsync(key);
        if (shipper == null)
        {
            return NotFound();
        }

        delta.Patch(shipper);
        await _context.SaveChangesAsync();
        return Updated(shipper);
    }

    /// <summary>
    /// Delete a shipper
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(int key)
    {
        var shipper = await _context.Shippers.FindAsync(key);
        if (shipper == null)
        {
            return NotFound();
        }

        _context.Shippers.Remove(shipper);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
