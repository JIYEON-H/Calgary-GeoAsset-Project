using GeoAsset.Api.Data;
using GeoAsset.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeoAsset.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InspectionLogsController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    // GET: api/InspectionLogs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InspectionLog>>> GetInspectionLogs()
    {
        return await _context.InspectionLogs.ToListAsync();
    }

    // GET: api/InspectionLogs/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<InspectionLog>> GetInspectionLog(int id)
    {
        var log = await _context.InspectionLogs.FindAsync(id);

        if (log is null)
        {
            return NotFound();
        }

        return log;
    }

    // GET: api/InspectionLogs/by-asset/5
    [HttpGet("by-asset/{assetId:int}")]
    public async Task<ActionResult<IEnumerable<InspectionLog>>> GetInspectionLogsByAsset(int assetId)
    {
        return await _context.InspectionLogs
            .Where(il => il.AssetId == assetId)
            .OrderByDescending(il => il.InspectionDate)
            .ToListAsync();
    }

    // POST: api/InspectionLogs
    [HttpPost]
    public async Task<ActionResult<InspectionLog>> PostInspectionLog(InspectionLog inspectionLog)
    {
        var asset = await _context.Assets.FindAsync(inspectionLog.AssetId);
        if (asset is null)
        {
            return BadRequest("Asset with the specified AssetId does not exist.");
        }

        _context.InspectionLogs.Add(inspectionLog);

        if(inspectionLog.InspectionDate > asset.LastUpdated){
            asset.LastUpdated = inspectionLog.InspectionDate;
        }
        
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInspectionLog), new { id = inspectionLog.Id }, inspectionLog);
    }

    // PUT: api/InspectionLogs/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> PutInspectionLog(int id, InspectionLog inspectionLog)
    {
        if (id != inspectionLog.Id)
        {
            return BadRequest();
        }

        if (!await _context.Assets.AnyAsync(a => a.Id == inspectionLog.AssetId))
        {
            return BadRequest("Asset with the specified AssetId does not exist.");
        }

        _context.Entry(inspectionLog).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!InspectionLogExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    // DELETE: api/InspectionLogs/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteInspectionLog(int id)
    {
        var log = await _context.InspectionLogs.FindAsync(id);
        if (log is null)
        {
            return NotFound();
        }

        _context.InspectionLogs.Remove(log);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool InspectionLogExists(int id)
    {
        return _context.InspectionLogs.Any(e => e.Id == id);
    }
}
