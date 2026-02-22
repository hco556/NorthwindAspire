using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;



public class SuppliersController : ODataController
{
    private readonly NorthwindContext _context;

    public SuppliersController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all suppliers with OData query support
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Supplier> Get()
    {
        return _context.Suppliers;
    }


    /// <summary>
    /// Create a new supplier
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Supplier>> Post([FromBody] Supplier supplier)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();
        return Created($"odata/suppliers({supplier.SupplierId})", supplier);
    }

    /// <summary>
    /// Update a supplier
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(int key, [FromBody] Delta<Supplier> delta)
    {
        var supplier = await _context.Suppliers.FindAsync(key);
        if (supplier == null)
        {
            return NotFound();
        }

        delta.Patch(supplier);
        await _context.SaveChangesAsync();
        return Updated(supplier);
    }

    /// <summary>
    /// Delete a supplier
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(int key)
    {
        var supplier = await _context.Suppliers.FindAsync(key);
        if (supplier == null)
        {
            return NotFound();
        }

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
