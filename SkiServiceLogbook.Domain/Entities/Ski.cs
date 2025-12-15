namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Yksittäinen suksi parissa (a tai b puoli)
/// </summary>
public class Ski
{
    /// <summary>Suksen yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Viittaus suksipariin</summary>
    public int SkiPairId { get; set; }
    
    /// <summary>Puoli: "a" tai "b"</summary>
    public string Side { get; set; } = string.Empty;
    
    /// <summary>Sarjanumero</summary>
    public string SerialNumber { get; set; } = string.Empty;
    
    /// <summary>Navigaatio-ominaisuus pariin</summary>
    public SkiPair SkiPair { get; set; } = null!;
}
