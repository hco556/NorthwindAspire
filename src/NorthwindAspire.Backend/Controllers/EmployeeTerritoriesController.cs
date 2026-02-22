using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;



public class EmployeeTerritoriesController : ODataController
{
    private readonly NorthwindContext _context;

    public EmployeeTerritoriesController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all employee territories with OData query support
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<EmployeeTerritory> Get()
    {
        return _context.EmployeeTerritories;
    }

    /// <summary>
    /// Get employee territory by EmployeeId and TerritoryId
    /// </summary>
    [HttpGet("{key}")]
    [EnableQuery]
    public async Task<ActionResult<EmployeeTerritory>> GetById([FromRoute] int key)
    {
        var employeeTerritory = await _context.EmployeeTerritories
            .FirstOrDefaultAsync(et => et.EmployeeId == key);
        if (employeeTerritory == null)
        {
            return NotFound();
        }
        return Ok(employeeTerritory);
    }

    /// <summary>
    /// Create a new employee territory
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmployeeTerritory>> Post([FromBody] EmployeeTerritory employeeTerritory)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.EmployeeTerritories.Add(employeeTerritory);
        await _context.SaveChangesAsync();
        return Created($"odata/employeeterritories({employeeTerritory.EmployeeId},'{employeeTerritory.TerritoryId}')", employeeTerritory);
    }

    /// <summary>
    /// Update an employee territory
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(int key, [FromBody] Delta<EmployeeTerritory> delta)
    {
        var employeeTerritory = await _context.EmployeeTerritories
            .FirstOrDefaultAsync(et => et.EmployeeId == key);
        if (employeeTerritory == null)
        {
            return NotFound();
        }

        delta.Patch(employeeTerritory);
        await _context.SaveChangesAsync();
        return Updated(employeeTerritory);
    }

    /// <summary>
    /// Delete an employee territory
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(int key)
    {
        var employeeTerritory = await _context.EmployeeTerritories
            .FirstOrDefaultAsync(et => et.EmployeeId == key);
        if (employeeTerritory == null)
        {
            return NotFound();
        }

        _context.EmployeeTerritories.Remove(employeeTerritory);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
