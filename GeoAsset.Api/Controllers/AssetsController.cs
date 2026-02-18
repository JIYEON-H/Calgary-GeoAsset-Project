using GeoAsset.Api.Data;
using GeoAsset.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeoAsset.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    // GET: api/Assets?includeInspectionLogs=true
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Asset>>> GetAssets([FromQuery] bool includeInspectionLogs = false)
    {
        var query = _context.Assets.AsQueryable();
        if (includeInspectionLogs)
        {
            query = query.Include(a => a.InspectionLogs);
        }
        // return await query.ToListAsync();
        return await query.OrderBy(a => a.Name).ToListAsync();
    }

    // GET: api/Assets/5?includeInspectionLogs=true
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Asset>> GetAsset(int id, [FromQuery] bool includeInspectionLogs = false)
    {
        var query = _context.Assets.AsQueryable();
        if (includeInspectionLogs)
        {
            query = query.Include(a => a.InspectionLogs);
        }
        var asset = await query.FirstOrDefaultAsync(a => a.Id == id);

        if (asset is null)
        {
            return NotFound();
        }

        return asset;
    }

    // POST: api/Assets
    [HttpPost]
    public async Task<ActionResult<Asset>> PostAsset(Asset asset)
    {
        if (asset.LastUpdated == default || asset.LastUpdated == null)
        {
        asset.LastUpdated = DateTime.UtcNow;
        }
        
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
    }

    // PUT: api/Assets/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutAsset(int id, Asset asset)
    {
        if (id != asset.Id)
        {
            return BadRequest();
        }

        if (asset.LastUpdated == default)
        {
            asset.LastUpdated = DateTime.UtcNow;
        }
        // asset.LastUpdated = DateTime.UtcNow;
        _context.Entry(asset).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AssetExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // DELETE: api/Assets/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteAsset(int id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset is null)
        {
            return NotFound();
        }

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool AssetExists(int id)
    {
        return _context.Assets.Any(e => e.Id == id);
    }
}
