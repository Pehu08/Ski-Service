using SkiServiceLogbook.Domain.Enums;

namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Ottelu turnausbracketeissa (single-elimination)
/// </summary>
public class Match
{
    /// <summary>Ottelun yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Viittaus testiin (jos ottelu on osa testiä)</summary>
    public int? TestEventId { get; set; }
    
    /// <summary>Turnauksen nimi</summary>
    public string TournamentName { get; set; } = string.Empty;
    
    /// <summary>Kierros (1 = ensimmäinen, 2 = toinen, jne.)</summary>
    public int Round { get; set; }
    
    /// <summary>Ottelunumero kierroksella</summary>
    public int MatchNumber { get; set; }
    
    /// <summary>Ensimmäinen suksipari</summary>
    public int? SkiPair1Id { get; set; }
    
    /// <summary>Toinen suksipari</summary>
    public int? SkiPair2Id { get; set; }
    
    /// <summary>Voittaja suksipari</summary>
    public int? WinnerSkiPairId { get; set; }
    
    /// <summary>Seuraava ottelu bracketeissa</summary>
    public int? NextMatchId { get; set; }
    
    /// <summary>Ottelun tila</summary>
    public MatchStatus Status { get; set; } = MatchStatus.Pending;
    
    /// <summary>Luontiaika</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>Navigaatio testiin</summary>
    public TestEvent? TestEvent { get; set; }
    
    /// <summary>Navigaatio ensimmäiseen suksipariin</summary>
    public SkiPair? SkiPair1 { get; set; }
    
    /// <summary>Navigaatio toiseen suksipariin</summary>
    public SkiPair? SkiPair2 { get; set; }
    
    /// <summary>Navigaatio voittajaan</summary>
    public SkiPair? WinnerSkiPair { get; set; }
}
