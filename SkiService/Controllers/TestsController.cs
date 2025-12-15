using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkiServiceLogbook.Infrastructure.Data;
using SkiServiceLogbook.Domain.Entities;
using SkiServiceLogbook.Domain.Enums;
using SkiServiceLogbook.Authorization;

namespace SkiServiceLogbook.Controllers;

/// <summary>
/// Testien hallinta
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationPolicies.ReadOnlyReports)] // Oletusarvoisesti lukuoikeus
public class TestsController : ControllerBase
{
    private readonly SkiServiceDbContext _context;

    public TestsController(SkiServiceDbContext context)
    {
        _context = context;
    }

    #region Testitapahtumat

    /// <summary>
    /// Hae kaikki testitapahtumat
    /// </summary>
    [HttpGet("events")]
    [AllowAnonymous] // Julkinen tiedon katselu
    public async Task<ActionResult<IEnumerable<TestEvent>>> GetTestEvents()
    {
        return await _context.TestEvents
            .Include(te => te.TestEventPairs)
                .ThenInclude(tep => tep.SkiPair)
            .OrderByDescending(te => te.EventDate)
            .ToListAsync();
    }

    /// <summary>
    /// Hae testitapahtuma ID:llä
    /// </summary>
    [HttpGet("events/{id}")]
    [AllowAnonymous] // Julkinen tiedon katselu
    public async Task<ActionResult<TestEvent>> GetTestEvent(int id)
    {
        var testEvent = await _context.TestEvents
            .Include(te => te.TestEventPairs)
                .ThenInclude(tep => tep.SkiPair)
            .Include(te => te.TestEventPairs)
                .ThenInclude(tep => tep.TestEventPairWaxes)
                    .ThenInclude(tepw => tepw.Wax)
                        .ThenInclude(w => w.WaxCategory)
            .FirstOrDefaultAsync(te => te.Id == id);

        if (testEvent == null)
        {
            return NotFound(new { message = "Testitapahtumaa ei löytynyt" });
        }

        return testEvent;
    }

    /// <summary>
    /// Luo uusi testitapahtuma
    /// </summary>
    [HttpPost("events")]
    [Authorize(Policy = AuthorizationPolicies.CanCreateTests)] // Admin, Huolto
    public async Task<ActionResult<TestEvent>> CreateTestEvent(TestEvent testEvent)
    {
        _context.TestEvents.Add(testEvent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTestEvent), new { id = testEvent.Id }, testEvent);
    }

    /// <summary>
    /// Päivitä testitapahtuma
    /// </summary>
    [HttpPut("events/{id}")]
    [Authorize(Policy = AuthorizationPolicies.CanCreateTests)] // Admin, Huolto
    public async Task<IActionResult> UpdateTestEvent(int id, TestEvent testEvent)
    {
        if (id != testEvent.Id)
        {
            return BadRequest(new { message = "ID ei täsmää" });
        }

        _context.Entry(testEvent).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.TestEvents.AnyAsync(e => e.Id == id))
            {
                return NotFound(new { message = "Testitapahtumaa ei löytynyt" });
            }
            throw;
        }

        return NoContent();
    }

    #endregion

    #region Testitulokset

    /// <summary>
    /// Lisää suksipari testiin
    /// </summary>
    [HttpPost("events/{eventId}/pairs")]
    [Authorize(Policy = AuthorizationPolicies.CanCreateTests)] // Admin, Huolto
    public async Task<ActionResult<TestEventPair>> AddSkiPairToTest(int eventId, TestEventPair testEventPair)
    {
        if (eventId != testEventPair.TestEventId)
        {
            return BadRequest(new { message = "Tapahtuma ID ei täsmää" });
        }

        // Tarkista että tapahtuma on olemassa
        if (!await _context.TestEvents.AnyAsync(te => te.Id == eventId))
        {
            return NotFound(new { message = "Testitapahtumaa ei löytynyt" });
        }

        // Tarkista että suksipari on olemassa
        if (!await _context.SkiPairs.AnyAsync(sp => sp.Id == testEventPair.SkiPairId))
        {
            return NotFound(new { message = "Suksiparia ei löytynyt" });
        }

        // Tarkista että suksiparia ei ole jo lisätty tähän testiin
        if (await _context.TestEventPairs.AnyAsync(tep => 
            tep.TestEventId == eventId && tep.SkiPairId == testEventPair.SkiPairId))
        {
            return BadRequest(new { message = "Suksipari on jo lisätty tähän testiin" });
        }

        _context.TestEventPairs.Add(testEventPair);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTestEvent), new { id = eventId }, testEventPair);
    }

    /// <summary>
    /// Päivitä testitulos
    /// </summary>
    [HttpPut("pairs/{id}")]
    [Authorize(Policy = AuthorizationPolicies.CanEnterResults)] // Admin, Huolto, Urheilija
    public async Task<IActionResult> UpdateTestEventPair(int id, TestEventPair testEventPair)
    {
        if (id != testEventPair.Id)
        {
            return BadRequest(new { message = "ID ei täsmää" });
        }

        _context.Entry(testEventPair).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.TestEventPairs.AnyAsync(e => e.Id == id))
            {
                return NotFound(new { message = "Testitulosta ei löytynyt" });
            }
            throw;
        }

        return NoContent();
    }

    /// <summary>
    /// Lisää voide testiparin voiteluun
    /// </summary>
    [HttpPost("pairs/{pairId}/waxes")]
    [Authorize(Policy = AuthorizationPolicies.CanCreateTests)] // Admin, Huolto
    public async Task<ActionResult<TestEventPairWax>> AddWaxToTestPair(int pairId, TestEventPairWax wax)
    {
        if (pairId != wax.TestEventPairId)
        {
            return BadRequest(new { message = "Testipari ID ei täsmää" });
        }

        _context.TestEventPairWaxes.Add(wax);
        await _context.SaveChangesAsync();

        return Ok(wax);
    }

    #endregion

    #region Turnaukset

    /// <summary>
    /// Hae kaikki turnaukset
    /// </summary>
    [HttpGet("tournaments")]
    [AllowAnonymous] // Julkinen tiedon katselu
    public async Task<ActionResult<IEnumerable<object>>> GetTournaments()
    {
        var tournaments = await _context.Matches
            .GroupBy(m => m.TournamentName)
            .Select(g => new
            {
                Name = g.Key,
                MatchCount = g.Count(),
                FirstMatchDate = g.Min(m => m.CreatedAt),
                Status = g.All(m => m.Status == MatchStatus.Completed) ? "Valmis" : 
                        g.Any(m => m.Status == MatchStatus.InProgress) ? "Käynnissä" : "Odottaa"
            })
            .OrderByDescending(t => t.FirstMatchDate)
            .ToListAsync();

        return Ok(tournaments);
    }

    /// <summary>
    /// Hae turnauksen ottelut
    /// </summary>
    [HttpGet("tournaments/{tournamentName}/matches")]
    [AllowAnonymous] // Julkinen tiedon katselu
    public async Task<ActionResult<IEnumerable<Match>>> GetTournamentMatches(string tournamentName)
    {
        var matches = await _context.Matches
            .Include(m => m.SkiPair1)
            .Include(m => m.SkiPair2)
            .Include(m => m.WinnerSkiPair)
            .Include(m => m.TestEvent)
            .Where(m => m.TournamentName == tournamentName)
            .OrderBy(m => m.Round)
            .ThenBy(m => m.MatchNumber)
            .ToListAsync();

        if (!matches.Any())
        {
            return NotFound(new { message = "Turnausta ei löytynyt" });
        }

        return matches;
    }

    #endregion
}
