using Microsoft.EntityFrameworkCore;
using SkiServiceLogbook.Infrastructure.Data;
using SkiServiceLogbook.Domain.Entities;
using SkiServiceLogbook.Domain.Enums;

namespace SkiServiceLogbook.Services;

/// <summary>
/// Palvelu single-elimination bracket -turnausten hallintaan
/// </summary>
public class BracketService
{
    private readonly SkiServiceDbContext _context;

    public BracketService(SkiServiceDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Luo uusi single-elimination turnaus annetuille suksipareille.
    /// Tukee automaattista bye-logiikkaa kun osallistujia ei ole kahden potenssi.
    /// </summary>
    /// <param name="tournamentName">Turnauksen nimi</param>
    /// <param name="skiPairIds">Osallistuvat suksipari ID:t</param>
    /// <param name="testEventId">Valinnainen testievent linkitys</param>
    /// <returns>Luodut ottelut</returns>
    public async Task<List<Match>> CreateTournamentAsync(
        string tournamentName, 
        List<int> skiPairIds, 
        int? testEventId = null)
    {
        if (skiPairIds == null || skiPairIds.Count < 2)
        {
            throw new ArgumentException("Turnaus vaatii vähintään 2 suksiparia");
        }

        // Selvitä turnauksen koko (seuraava kahden potenssi)
        int tournamentSize = GetNextPowerOfTwo(skiPairIds.Count);
        int byeCount = tournamentSize - skiPairIds.Count;

        var matches = new List<Match>();
        var skiPairQueue = new Queue<int?>(skiPairIds.Cast<int?>());

        // Lisää bye:t jonoon (null = bye)
        for (int i = 0; i < byeCount; i++)
        {
            skiPairQueue.Enqueue(null);
        }

        // Luo ensimmäisen kierroksen ottelut
        int matchNumber = 1;
        var currentRoundMatches = new List<Match>();

        while (skiPairQueue.Count >= 2)
        {
            var pair1 = skiPairQueue.Dequeue();
            var pair2 = skiPairQueue.Dequeue();

            var match = new Match
            {
                TournamentName = tournamentName,
                Round = 1,
                MatchNumber = matchNumber++,
                SkiPair1Id = pair1,
                SkiPair2Id = pair2,
                TestEventId = testEventId,
                Status = MatchStatus.Pending
            };

            // Jos toinen on bye, automaattisesti voitto ensimmäiselle
            if (pair2 == null && pair1 != null)
            {
                match.WinnerSkiPairId = pair1;
                match.Status = MatchStatus.Completed;
            }
            // Jos molemmat ovat bye (ei pitäisi tapahtua)
            else if (pair1 == null && pair2 == null)
            {
                match.Status = MatchStatus.Pending; // Käytetään Pending, koska Cancelled ei ole enumerissa
            }

            currentRoundMatches.Add(match);
            matches.Add(match);
        }

        // Luo seuraavat kierrokset kunnes on vain yksi ottelu (finaali)
        int currentRound = 1;
        while (currentRoundMatches.Count > 1)
        {
            var nextRoundMatches = new List<Match>();
            matchNumber = 1;
            currentRound++;

            for (int i = 0; i < currentRoundMatches.Count; i += 2)
            {
                var nextMatch = new Match
                {
                    TournamentName = tournamentName,
                    Round = currentRound,
                    MatchNumber = matchNumber++,
                    TestEventId = testEventId,
                    Status = MatchStatus.Pending
                };

                // Linkitä edellisen kierroksen ottelut seuraavaan otteluun
                currentRoundMatches[i].NextMatchId = nextMatch.Id;
                if (i + 1 < currentRoundMatches.Count)
                {
                    currentRoundMatches[i + 1].NextMatchId = nextMatch.Id;
                }

                nextRoundMatches.Add(nextMatch);
                matches.Add(nextMatch);
            }

            currentRoundMatches = nextRoundMatches;
        }

        // Tallenna tietokantaan
        await _context.Matches.AddRangeAsync(matches);
        await _context.SaveChangesAsync();

        // Päivitä NextMatchId:t nyt kun ID:t on generoitu
        await UpdateNextMatchReferencesAsync(matches);

        return matches;
    }

    /// <summary>
    /// Päivittää ottelun tuloksen ja eteenpäin viemisen
    /// </summary>
    public async Task<Match> UpdateMatchResultAsync(
        int matchId, 
        int winnerSkiPairId, 
        decimal? skiPair1Score = null, 
        decimal? skiPair2Score = null)
    {
        var match = await _context.Matches
            .FirstOrDefaultAsync(m => m.Id == matchId);

        if (match == null)
        {
            throw new ArgumentException($"Ottelua ID {matchId} ei löydy");
        }

        if (match.SkiPair1Id != winnerSkiPairId && match.SkiPair2Id != winnerSkiPairId)
        {
            throw new ArgumentException("Voittajan täytyy olla yksi ottelun osallistujista");
        }

        match.WinnerSkiPairId = winnerSkiPairId;
        // Huom: Score-kenttiä ei ole Match-entityssä, tulostieto on MeasuredResult:ssa TestEventPair:ssa
        match.Status = MatchStatus.Completed;

        // Jos on seuraava ottelu, vie voittaja sinne
        if (match.NextMatchId.HasValue)
        {
            var nextMatch = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == match.NextMatchId.Value);

            if (nextMatch != null)
            {
                // Lisää voittaja seuraavaan otteluun
                if (nextMatch.SkiPair1Id == null)
                {
                    nextMatch.SkiPair1Id = winnerSkiPairId;
                }
                else if (nextMatch.SkiPair2Id == null)
                {
                    nextMatch.SkiPair2Id = winnerSkiPairId;
                }
            }
        }

        await _context.SaveChangesAsync();
        return match;
    }

    /// <summary>
    /// Hakee turnauksen kaikki ottelut
    /// </summary>
    public async Task<List<Match>> GetTournamentMatchesAsync(string tournamentName)
    {
        return await _context.Matches
            .Include(m => m.SkiPair1)
            .Include(m => m.SkiPair2)
            .Include(m => m.WinnerSkiPair)
            .Include(m => m.TestEvent)
            .Where(m => m.TournamentName == tournamentName)
            .OrderBy(m => m.Round)
            .ThenBy(m => m.MatchNumber)
            .ToListAsync();
    }

    /// <summary>
    /// Hakee tietyn kierroksen ottelut
    /// </summary>
    public async Task<List<Match>> GetRoundMatchesAsync(string tournamentName, int round)
    {
        return await _context.Matches
            .Include(m => m.SkiPair1)
            .Include(m => m.SkiPair2)
            .Include(m => m.WinnerSkiPair)
            .Where(m => m.TournamentName == tournamentName && m.Round == round)
            .OrderBy(m => m.MatchNumber)
            .ToListAsync();
    }

    /// <summary>
    /// Palauttaa seuraavan kahden potenssin
    /// </summary>
    private int GetNextPowerOfTwo(int n)
    {
        if (n <= 1) return 2;
        
        int power = 1;
        while (power < n)
        {
            power *= 2;
        }
        return power;
    }

    /// <summary>
    /// Päivittää NextMatchId-viittaukset tietokantaan
    /// </summary>
    private async Task UpdateNextMatchReferencesAsync(List<Match> matches)
    {
        var matchesByRoundAndNumber = matches
            .ToDictionary(m => (m.Round, m.MatchNumber), m => m);

        foreach (var match in matches.Where(m => m.Round < matches.Max(x => x.Round)))
        {
            int nextRound = match.Round + 1;
            int nextMatchNumber = (match.MatchNumber + 1) / 2;

            if (matchesByRoundAndNumber.TryGetValue((nextRound, nextMatchNumber), out var nextMatch))
            {
                match.NextMatchId = nextMatch.Id;
            }
        }

        await _context.SaveChangesAsync();
    }
}
