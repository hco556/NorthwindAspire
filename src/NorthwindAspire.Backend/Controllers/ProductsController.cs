using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;

[ApiController]
[Route("odata/[controller]")]
public class ProductsController : ODataController
{
    private readonly NorthwindContext _context;

    public ProductsController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all products with OData query support
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Product> Get()
    {
        return _context.Products;
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{key}")]
    [EnableQuery]
    public async Task<ActionResult<Product>> GetById(int key)
    {
        var product = await _context.Products.FindAsync(key);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    public async Task<ActionResult<Product>> Post([FromBody] Product product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return Created($"odata/products({product.ProductId})", product);
    }

    /// <summary>
    /// Update a product
    /// </summary>
    public async Task<ActionResult> Patch(int key, [FromBody] Delta<Product> delta)
    {
        var product = await _context.Products.FindAsync(key);
        if (product == null)
        {
            return NotFound();
        }

        delta.Patch(product);
        await _context.SaveChangesAsync();
        return Updated(product);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    public async Task<ActionResult> Delete(int key)
    {
        var product = await _context.Products.FindAsync(key);
        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
