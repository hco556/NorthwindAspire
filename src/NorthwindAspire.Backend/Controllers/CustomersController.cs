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
public class CustomersController : ODataController
{
    private readonly NorthwindContext _context;

    public CustomersController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Customer> Get()
    {
        return _context.Customers;
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    [HttpGet("{key}")]
    [EnableQuery]
    public async Task<ActionResult<Customer>> GetById(string key)
    {
        var customer = await _context.Customers.FindAsync(key);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Customer>> Post([FromBody] Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return Created($"odata/customers('{customer.CustomerId}')", customer);
    }

    /// <summary>
    /// Update a customer
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(string key, [FromBody] Delta<Customer> delta)
    {
        var customer = await _context.Customers.FindAsync(key);
        if (customer == null)
        {
            return NotFound();
        }

        delta.Patch(customer);
        await _context.SaveChangesAsync();
        return Updated(customer);
    }

    /// <summary>
    /// Delete a customer
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(string key)
    {
        var customer = await _context.Customers.FindAsync(key);
        if (customer == null)
        {
            return NotFound();
        }

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
