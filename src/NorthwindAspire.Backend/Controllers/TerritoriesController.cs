using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;



public class TerritoriesController : ODataController
{
    private readonly NorthwindContext _context;

    public TerritoriesController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all territories with OData query support
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Territory> Get()
    {
        return _context.Territories;
    }

    /// <summary>
    /// Create a new territory
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Territory>> Post([FromBody] Territory territory)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Territories.Add(territory);
        await _context.SaveChangesAsync();
        return Created($"odata/territories('{territory.TerritoryId}')", territory);
    }

    /// <summary>
    /// Update a territory
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(string key, [FromBody] Delta<Territory> delta)
    {
        var territory = await _context.Territories.FindAsync(key);
        if (territory == null)
        {
            return NotFound();
        }

        delta.Patch(territory);
        await _context.SaveChangesAsync();
        return Updated(territory);
    }

    /// <summary>
    /// Delete a territory
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(string key)
    {
        var territory = await _context.Territories.FindAsync(key);
        if (territory == null)
        {
            return NotFound();
        }

        _context.Territories.Remove(territory);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
