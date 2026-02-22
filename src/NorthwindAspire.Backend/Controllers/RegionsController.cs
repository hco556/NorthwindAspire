using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;

public class RegionsController : ODataController
{
    private readonly NorthwindContext _context;

    public RegionsController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all regions with OData query support
    /// </summary>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Region> Get()
    {
        return _context.Regions;
    }


    /// <summary>
    /// Create a new region
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Region>> Post([FromBody] Region region)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Regions.Add(region);
        await _context.SaveChangesAsync();
        return Created($"odata/regions({region.RegionId})", region);
    }

    /// <summary>
    /// Update a region
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(int key, [FromBody] Delta<Region> delta)
    {
        var region = await _context.Regions.FindAsync(key);
        if (region == null)
        {
            return NotFound();
        }

        delta.Patch(region);
        await _context.SaveChangesAsync();
        return Updated(region);
    }

    /// <summary>
    /// Delete a region
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(int key)
    {
        var region = await _context.Regions.FindAsync(key);
        if (region == null)
        {
            return NotFound();
        }

        _context.Regions.Remove(region);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
