namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Voideluaineiden kategoria (luisto, pito, pinnoite)
/// </summary>
public class WaxCategory
{
    /// <summary>Kategorian yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Kategorian nimi (esim. "Luistovoide")</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Kuvaus</summary>
    public string? Description { get; set; }
    
    /// <summary>Näyttöjärjestys UI:ssa</summary>
    public int DisplayOrder { get; set; }
    
    /// <summary>Tämän kategorian voiteet</summary>
    public ICollection<Wax> Waxes { get; set; } = new List<Wax>();
}
