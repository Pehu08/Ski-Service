using Microsoft.EntityFrameworkCore;
using SkiServiceLogbook.Infrastructure.Data;
using SkiServiceLogbook.Domain.Entities;

namespace SkiServiceLogbook.Services;

/// <summary>
/// Hakupalvelu voiteiden ja testien etsimiseen olosuhdetoleransseilla
/// </summary>
public class SearchService
{
    private readonly SkiServiceDbContext _context;

    public SearchService(SkiServiceDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Hakee voiteet annetuille olosuhteille toleranssilla
    /// </summary>
    /// <param name="temperature">Lämpötila (°C)</param>
    /// <param name="humidity">Kosteus (%)</param>
    /// <param name="temperatureTolerance">Lämpötilatoleranssi (± °C)</param>
    /// <param name="humidityTolerance">Kosteustoleranssi (± %)</param>
    /// <returns>Sopivat voiteet</returns>
    public async Task<List<Wax>> SearchWaxesByConditionsAsync(
        decimal temperature,
        decimal? humidity = null,
        decimal temperatureTolerance = 2.0m,
        decimal? humidityTolerance = 10.0m)
    {
        var minTemp = temperature - temperatureTolerance;
        var maxTemp = temperature + temperatureTolerance;

        var query = _context.Waxes
            .Include(w => w.WaxCategory)
            .Where(w => w.IsActive);

        // Lämpötilasuodatus
        query = query.Where(w =>
            (w.MinTemperature == null || w.MinTemperature <= maxTemp) &&
            (w.MaxTemperature == null || w.MaxTemperature >= minTemp));

        // Kosteussuodatus jos annettu
        if (humidity.HasValue && humidityTolerance.HasValue)
        {
            var minHum = humidity.Value - humidityTolerance.Value;
            var maxHum = humidity.Value + humidityTolerance.Value;

            query = query.Where(w =>
                w.MinHumidity == null || w.MaxHumidity == null ||
                (w.MinHumidity <= maxHum && w.MaxHumidity >= minHum));
        }

        return await query
            .OrderBy(w => w.WaxCategory.DisplayOrder)
            .ThenBy(w => w.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Hakee voiteluohjeet tekstihaulla
    /// </summary>
    public async Task<List<WaxInstruction>> SearchWaxInstructionsAsync(string searchText)
    {
        var query = _context.WaxInstructions
            .Where(wi =>
                wi.Title.Contains(searchText) ||
                wi.Instructions.Contains(searchText) ||
                (wi.Conditions != null && wi.Conditions.Contains(searchText)));

        return await query
            .OrderByDescending(wi => wi.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Hakee aikaisemmat testitapahtumat samankaltaisille olosuhteille
    /// </summary>
    public async Task<List<TestEvent>> SearchSimilarTestEventsAsync(
        decimal temperature,
        decimal? humidity = null,
        string? snowCondition = null,
        decimal temperatureTolerance = 3.0m,
        decimal? humidityTolerance = 15.0m)
    {
        var minTemp = temperature - temperatureTolerance;
        var maxTemp = temperature + temperatureTolerance;

        var query = _context.TestEvents
            .Include(te => te.TestEventPairs)
                .ThenInclude(tep => tep.SkiPair)
            .Include(te => te.TestEventPairs)
                .ThenInclude(tep => tep.TestEventPairWaxes)
                    .ThenInclude(tepw => tepw.Wax)
            .Where(te => 
                te.AirTemperature >= minTemp &&
                te.AirTemperature <= maxTemp);

        // Kosteussuodatus
        if (humidity.HasValue && humidityTolerance.HasValue)
        {
            var minHum = humidity.Value - humidityTolerance.Value;
            var maxHum = humidity.Value + humidityTolerance.Value;

            query = query.Where(te =>
                te.Humidity == null ||
                (te.Humidity >= minHum && te.Humidity <= maxHum));
        }

        // Lumiolosuhteet
        if (!string.IsNullOrEmpty(snowCondition))
        {
            query = query.Where(te => 
                EF.Functions.Like(te.SnowCondition, $"%{snowCondition}%"));
        }

        return await query
            .OrderByDescending(te => te.EventDate)
            .Take(20)
            .ToListAsync();
    }

    /// <summary>
    /// Hakee parhaat voiteyhdistelmät samankaltaisissa olosuhteissa
    /// </summary>
    public async Task<List<TestEventPairWaxResult>> GetBestWaxCombinationsAsync(
        decimal temperature,
        decimal? humidity = null,
        decimal temperatureTolerance = 3.0m,
        decimal? humidityTolerance = 15.0m,
        int topResults = 10)
    {
        var minTemp = temperature - temperatureTolerance;
        var maxTemp = temperature + temperatureTolerance;

        var query = _context.TestEventPairs
            .Include(tep => tep.TestEvent)
            .Include(tep => tep.SkiPair)
            .Include(tep => tep.TestEventPairWaxes)
                .ThenInclude(tepw => tepw.Wax)
                    .ThenInclude(w => w.WaxCategory)
            .Where(tep =>
                tep.TestEvent.AirTemperature >= minTemp &&
                tep.TestEvent.AirTemperature <= maxTemp &&
                tep.MeasuredResult != null &&
                tep.Ranking != null);

        if (humidity.HasValue && humidityTolerance.HasValue)
        {
            var minHum = humidity.Value - humidityTolerance.Value;
            var maxHum = humidity.Value + humidityTolerance.Value;

            query = query.Where(tep =>
                tep.TestEvent.Humidity == null ||
                (tep.TestEvent.Humidity >= minHum && tep.TestEvent.Humidity <= maxHum));
        }

        var results = await query
            .OrderBy(tep => tep.Ranking)
            .ThenBy(tep => tep.MeasuredResult) // Käytetään MeasuredResult TimeSeconds:in sijaan
            .Take(topResults)
            .ToListAsync();

        return results.Select(tep => new TestEventPairWaxResult
        {
            TestEventPair = tep,
            Waxes = tep.TestEventPairWaxes
                .OrderBy(tepw => tepw.LayerOrder)
                .Select(tepw => tepw.Wax)
                .ToList()
        }).ToList();
    }

    /// <summary>
    /// Hakee suksiparin testaushistorian
    /// </summary>
    public async Task<List<TestEventPair>> GetSkiPairHistoryAsync(
        int skiPairId,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _context.TestEventPairs
            .Include(tep => tep.TestEvent)
            .Include(tep => tep.TestEventPairWaxes)
                .ThenInclude(tepw => tepw.Wax)
                    .ThenInclude(w => w.WaxCategory)
            .Where(tep => tep.SkiPairId == skiPairId);

        if (fromDate.HasValue)
        {
            query = query.Where(tep => tep.TestEvent.EventDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(tep => tep.TestEvent.EventDate <= toDate.Value);
        }

        return await query
            .OrderByDescending(tep => tep.TestEvent.EventDate)
            .ToListAsync();
    }
}

/// <summary>
/// Voiteyhdistelmän testitulos
/// </summary>
public class TestEventPairWaxResult
{
    public TestEventPair TestEventPair { get; set; } = null!;
    public List<Wax> Waxes { get; set; } = new List<Wax>();
}
