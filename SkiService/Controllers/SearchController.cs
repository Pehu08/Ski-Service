using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiServiceLogbook.Infrastructure.Data;
using SkiServiceLogbook.Services;
using SkiServiceLogbook.Authorization;

namespace SkiServiceLogbook.Controllers;

/// <summary>
/// Haku- ja suosittelutoiminnot
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Julkinen hakutoiminnallisuus
public class SearchController : ControllerBase
{
    private readonly SearchService _searchService;

    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }

    /// <summary>
    /// Hae sopivat voiteet olosuhteille
    /// </summary>
    /// <param name="temperature">Lämpötila °C</param>
    /// <param name="humidity">Ilmankosteus %</param>
    /// <param name="temperatureTolerance">Lämpötilatoleranssi ±°C (oletus 2.0)</param>
    /// <param name="humidityTolerance">Kosteustoleranssi ±% (oletus 10.0)</param>
    [HttpGet("waxes")]
    public async Task<IActionResult> SearchWaxes(
        [FromQuery] decimal temperature,
        [FromQuery] decimal? humidity = null,
        [FromQuery] decimal temperatureTolerance = 2.0m,
        [FromQuery] decimal? humidityTolerance = 10.0m)
    {
        var waxes = await _searchService.SearchWaxesByConditionsAsync(
            temperature, humidity, temperatureTolerance, humidityTolerance);

        return Ok(new
        {
            queryConditions = new
            {
                temperature,
                humidity,
                temperatureTolerance,
                humidityTolerance
            },
            resultsCount = waxes.Count,
            waxes
        });
    }

    /// <summary>
    /// Hae voiteluohjeet tekstihaulla
    /// </summary>
    [HttpGet("instructions")]
    public async Task<IActionResult> SearchInstructions(
        [FromQuery] string searchText = "")
    {
        var instructions = await _searchService.SearchWaxInstructionsAsync(searchText);

        return Ok(new
        {
            queryConditions = new
            {
                searchText
            },
            resultsCount = instructions.Count,
            instructions
        });
    }

    /// <summary>
    /// Hae samankaltaiset testitapahtumat
    /// </summary>
    [HttpGet("similar-tests")]
    public async Task<IActionResult> SearchSimilarTests(
        [FromQuery] decimal temperature,
        [FromQuery] decimal? humidity = null,
        [FromQuery] string? snowCondition = null,
        [FromQuery] decimal temperatureTolerance = 3.0m,
        [FromQuery] decimal? humidityTolerance = 15.0m)
    {
        var tests = await _searchService.SearchSimilarTestEventsAsync(
            temperature, humidity, snowCondition, temperatureTolerance, humidityTolerance);

        return Ok(new
        {
            queryConditions = new
            {
                temperature,
                humidity,
                snowCondition,
                temperatureTolerance,
                humidityTolerance
            },
            resultsCount = tests.Count,
            tests
        });
    }

    /// <summary>
    /// Hae parhaat voiteyhdistelmät samankaltaisissa olosuhteissa
    /// </summary>
    [HttpGet("best-combinations")]
    public async Task<IActionResult> GetBestCombinations(
        [FromQuery] decimal temperature,
        [FromQuery] decimal? humidity = null,
        [FromQuery] decimal temperatureTolerance = 3.0m,
        [FromQuery] decimal? humidityTolerance = 15.0m,
        [FromQuery] int top = 10)
    {
        var results = await _searchService.GetBestWaxCombinationsAsync(
            temperature, humidity, temperatureTolerance, humidityTolerance, top);

        return Ok(new
        {
            queryConditions = new
            {
                temperature,
                humidity,
                temperatureTolerance,
                humidityTolerance
            },
            resultsCount = results.Count,
            results = results.Select(r => new
            {
                skiPair = new
                {
                    pairNumber = r.TestEventPair.SkiPair.PairNumber,
                    brand = r.TestEventPair.SkiPair.Brand,
                    model = r.TestEventPair.SkiPair.Model
                },
                testEvent = new
                {
                    name = r.TestEventPair.TestEvent.Name,
                    date = r.TestEventPair.TestEvent.EventDate,
                    location = r.TestEventPair.TestEvent.Location,
                    airTemperature = r.TestEventPair.TestEvent.AirTemperature,
                    humidity = r.TestEventPair.TestEvent.Humidity,
                    snowCondition = r.TestEventPair.TestEvent.SnowCondition
                },
                result = new
                {
                    measuredResult = r.TestEventPair.MeasuredResult,
                    ranking = r.TestEventPair.Ranking
                },
                waxes = r.Waxes.Select(w => new
                {
                    id = w.Id,
                    name = w.Name,
                    manufacturer = w.Manufacturer,
                    category = w.WaxCategory.Name
                })
            })
        });
    }

    /// <summary>
    /// Hae suksiparin testaushistoria
    /// </summary>
    [HttpGet("skipair-history/{skiPairId}")]
    public async Task<IActionResult> GetSkiPairHistory(
        int skiPairId,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var history = await _searchService.GetSkiPairHistoryAsync(skiPairId, fromDate, toDate);

        return Ok(new
        {
            skiPairId,
            dateRange = new { fromDate, toDate },
            testsCount = history.Count,
            history = history.Select(h => new
            {
                testEvent = new
                {
                    name = h.TestEvent.Name,
                    date = h.TestEvent.EventDate,
                    location = h.TestEvent.Location,
                    airTemperature = h.TestEvent.AirTemperature,
                    snowCondition = h.TestEvent.SnowCondition
                },
                result = new
                {
                    measuredResult = h.MeasuredResult,
                    ranking = h.Ranking,
                    notes = h.Notes
                },
                waxes = h.TestEventPairWaxes.Select(tepw => new
                {
                    layerOrder = tepw.LayerOrder,
                    waxName = tepw.Wax.Name,
                    manufacturer = tepw.Wax.Manufacturer,
                    category = tepw.Wax.WaxCategory.Name
                    // Huom: TestEventPairWax:ssa ei ole Notes-kenttää
                })
            })
        });
    }
}
