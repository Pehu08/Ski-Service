namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Voiteluohje (yksinkertainen muistiinpano voiteluista)
/// </summary>
public class WaxInstruction
{
    /// <summary>Ohjeen yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Ohjeen otsikko</summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>Voiteluohjeet</summary>
    public string Instructions { get; set; } = string.Empty;
    
    /// <summary>Soveltuvat olosuhteet</summary>
    public string? Conditions { get; set; }
    
    /// <summary>Luontiaika</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
