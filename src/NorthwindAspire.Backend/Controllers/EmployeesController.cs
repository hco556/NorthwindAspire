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
public class EmployeesController : ODataController
{
    private readonly NorthwindContext _context;

    public EmployeesController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all employees with OData query support
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Employee> Get()
    {
        return _context.Employees;
    }

    /// <summary>
    /// Get employee by ID
    /// </summary>
    [HttpGet("{key}")]
    [EnableQuery]
    public async Task<ActionResult<Employee>> GetById(int key)
    {
        var employee = await _context.Employees.FindAsync(key);
        if (employee == null)
        {
            return NotFound();
        }
        return Ok(employee);
    }

    /// <summary>
    /// Create a new employee
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Employee>> Post([FromBody] Employee employee)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return Created($"odata/employees({employee.EmployeeId})", employee);
    }

    /// <summary>
    /// Update an employee
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(int key, [FromBody] Delta<Employee> delta)
    {
        var employee = await _context.Employees.FindAsync(key);
        if (employee == null)
        {
            return NotFound();
        }

        delta.Patch(employee);
        await _context.SaveChangesAsync();
        return Updated(employee);
    }

    /// <summary>
    /// Delete an employee
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(int key)
    {
        var employee = await _context.Employees.FindAsync(key);
        if (employee == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
