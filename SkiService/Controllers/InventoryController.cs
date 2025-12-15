using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiServiceLogbook.Infrastructure.Data;
using SkiServiceLogbook.Domain.Entities;
using SkiServiceLogbook.Authorization;

namespace SkiServiceLogbook.Controllers;

/// <summary>
/// Varaston hallinta: suksiparit ja voiteet
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.ReadOnlyReports)] // Oletusarvoisesti lukuoikeus
public class InventoryController : ControllerBase
{
    private readonly SkiServiceDbContext _context;

    public InventoryController(SkiServiceDbContext context)
    {
        _context = context;
    }

    #region Suksiparit

    /// <summary>
    /// Hae kaikki suksiparit
    /// </summary>
    [HttpGet("skipairs")]
    [AllowAnonymous] // Julkinen tiedon katselu
    public async Task<ActionResult<IEnumerable<SkiPair>>> GetSkiPairs()
    {
        return await _context.SkiPairs
            .Include(sp => sp.Skis)
            .Where(sp => sp.IsActive)
            .OrderBy(sp => sp.PairNumber)
            .ToListAsync();
    }

    /// <summary>
    /// Hae suksipari ID:llä
    /// </summary>
    [HttpGet("skipairs/{id}")]
    [AllowAnonymous] // Julkinen tiedon katselu
    public async Task<ActionResult<SkiPair>> GetSkiPair(int id)
    {
        var skiPair = await _context.SkiPairs
            .Include(sp => sp.Skis)
            .Include(sp => sp.TestEventPairs)
                .ThenInclude(tep => tep.TestEvent)
            .FirstOrDefaultAsync(sp => sp.Id == id);

        if (skiPair == null)
        {
            return NotFound(new { message = "Suksiparia ei löytynyt" });
        }

        return skiPair;
    }

    /// <summary>
    /// Luo uusi suksipari
    /// </summary>
    [HttpPost("skipairs")]
    [Authorize(Policy = AuthorizationPolicies.CanManageInventory)] // Vain Admin
    public async Task<ActionResult<SkiPair>> CreateSkiPair(SkiPair skiPair)
    {
        // Tarkista että parinumeroa ei ole jo käytössä
        if (await _context.SkiPairs.AnyAsync(sp => sp.PairNumber == skiPair.PairNumber))
        {
            return BadRequest(new { message = $"Parinumero {skiPair.PairNumber} on jo käytössä" });
        }

        _context.SkiPairs.Add(skiPair);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSkiPair), new { id = skiPair.Id }, skiPair);
    }

    /// <summary>
    /// Päivitä suksipari
    /// </summary>
    [HttpPut("skipairs/{id}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageInventory)] // Vain Admin
    public async Task<IActionResult> UpdateSkiPair(int id, SkiPair skiPair)
    {
        if (id != skiPair.Id)
        {
            return BadRequest(new { message = "ID ei täsmää" });
        }

        _context.Entry(skiPair).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.SkiPairs.AnyAsync(e => e.Id == id))
            {
                return NotFound(new { message = "Suksiparia ei löytynyt" });
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Poista suksipari (soft delete)
    /// </summary>
    [HttpDelete("skipairs/{id}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageInventory)] // Vain Admin
    public async Task<IActionResult> DeleteSkiPair(int id)
    {
        var skiPair = await _context.SkiPairs.FindAsync(id);
        if (skiPair == null)
        {
            return NotFound(new { message = "Suksiparia ei löytynyt" });
        }

        skiPair.IsActive = false;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    #endregion

    #region Voiteet

    /// <summary>
    /// Hae kaikki voiteet
    /// </summary>
    [HttpGet("waxes")]
    [AllowAnonymous] // Julkinen tiedon katselu
    public async Task<ActionResult<IEnumerable<Wax>>> GetWaxes()
    {
        return await _context.Waxes
            .Include(w => w.WaxCategory)
            .Where(w => w.IsActive)
            .OrderBy(w => w.WaxCategory.DisplayOrder)
            .ThenBy(w => w.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Hae voide ID:llä
    /// </summary>
    [HttpGet("waxes/{id}")]
    [AllowAnonymous] // Julkinen tiedon katselu
    public async Task<ActionResult<Wax>> GetWax(int id)
    {
        var wax = await _context.Waxes
            .Include(w => w.WaxCategory)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (wax == null)
        {
            return NotFound(new { message = "Voidetta ei löytynyt" });
        }

        return wax;
    }

    /// <summary>
    /// Luo uusi voide
    /// </summary>
    [HttpPost("waxes")]
    [Authorize(Policy = AuthorizationPolicies.CanManageInventory)] // Vain Admin
    public async Task<ActionResult<Wax>> CreateWax(Wax wax)
    {
        _context.Waxes.Add(wax);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWax), new { id = wax.Id }, wax);
    }

    /// <summary>
    /// Päivitä voide
    /// </summary>
    [HttpPut("waxes/{id}")]
    [Authorize(Policy = AuthorizationPolicies.CanManageInventory)] // Vain Admin
    public async Task<IActionResult> UpdateWax(int id, Wax wax)
    {
        if (id != wax.Id)
        {
            return BadRequest(new { message = "ID ei täsmää" });
        }

        _context.Entry(wax).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Waxes.AnyAsync(e => e.Id == id))
            {
                return NotFound(new { message = "Voidetta ei löytynyt" });
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Hae voitekategoriat
    /// </summary>
    [HttpGet("waxcategories")]
    [AllowAnonymous] // Julkinen tiedon katselu
    public async Task<ActionResult<IEnumerable<WaxCategory>>> GetWaxCategories()
    {
        return await _context.WaxCategories
            .OrderBy(wc => wc.DisplayOrder)
            .ToListAsync();
    }

    #endregion
}
