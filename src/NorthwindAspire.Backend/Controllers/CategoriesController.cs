using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using NorthwindAspire.Backend.Data;
using NorthwindAspire.Backend.Models;

namespace NorthwindAspire.Backend.Controllers;



public class CategoriesController : ODataController
{
    private readonly NorthwindContext _context;

    public CategoriesController(NorthwindContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all categories with OData query support
    /// </summary>
    /// <remarks>
    /// OData Query Examples:
    /// - /odata/categories?$filter=categoryName eq 'Beverages'
    /// - /odata/categories?$select=categoryId,categoryName
    /// - /odata/categories?$expand=products
    /// - /odata/categories?$orderby=categoryName&$top=10&$skip=5
    /// </remarks>
    [HttpGet]
    [EnableQuery(PageSize = 100)]
    public IQueryable<Category> Get()
    {
        return _context.Categories;
    }


    /// <summary>
    /// Create a new category
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Category>> Post([FromBody] Category category)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Created($"odata/categories({category.CategoryId})", category);
    }

    /// <summary>
    /// Update a category
    /// </summary>
    [HttpPatch("{key}")]
    public async Task<ActionResult> Patch(int key, [FromBody] Delta<Category> delta)
    {
        var category = await _context.Categories.FindAsync(key);
        if (category == null)
        {
            return NotFound();
        }

        delta.Patch(category);
        await _context.SaveChangesAsync();
        return Updated(category);
    }

    /// <summary>
    /// Delete a category
    /// </summary>
    [HttpDelete("{key}")]
    public async Task<ActionResult> Delete(int key)
    {
        var category = await _context.Categories.FindAsync(key);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
