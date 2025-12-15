namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Suksipari jossa kaksi suksea (a ja b)
/// </summary>
public class SkiPair
{
    /// <summary>Suksipar in yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Parinumero (esim. 1-10), uniikki</summary>
    public int PairNumber { get; set; }
    
    /// <summary>Suksien merkki (esim. Fischer, Atomic)</summary>
    public string Brand { get; set; } = string.Empty;
    
    /// <summary>Malli</summary>
    public string Model { get; set; } = string.Empty;
    
    /// <summary>Pituus senttimetreinä</summary>
    public int Length { get; set; }
    
    /// <summary>Onko suksipari käytössä (soft delete)</summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>Luontiaika</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>Muistiinpanot</summary>
    public string? Notes { get; set; }
    
    /// <summary>Parin yksittäiset sukset (a ja b)</summary>
    public ICollection<Ski> Skis { get; set; } = new List<Ski>();
    
    /// <summary>Testitulokset joissa tämä pari on mukana</summary>
    public ICollection<TestEventPair> TestEventPairs { get; set; } = new List<TestEventPair>();
    
    /// <summary>Ottelut joissa tämä pari on Ski1</summary>
    public ICollection<Match> MatchesAsSkiPair1 { get; set; } = new List<Match>();
    
    /// <summary>Ottelut joissa tämä pari on Ski2</summary>
    public ICollection<Match> MatchesAsSkiPair2 { get; set; } = new List<Match>();
    
    /// <summary>Ottelut joissa tämä pari voitti</summary>
    public ICollection<Match> MatchesAsWinner { get; set; } = new List<Match>();
}
