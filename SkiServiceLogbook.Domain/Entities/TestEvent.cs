namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Testitapahtuma (esim. "Vuokatti testit 2024")
/// </summary>
public class TestEvent
{
    /// <summary>Tapahtuman yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Tapahtuman nimi</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Tapahtuman päivämäärä</summary>
    public DateTime EventDate { get; set; }
    
    /// <summary>Testipaikka</summary>
    public string Location { get; set; } = string.Empty;
    
    /// <summary>Ilman lämpötila (Celsius)</summary>
    public decimal? AirTemperature { get; set; }
    
    /// <summary>Lumen lämpötila (Celsius)</summary>
    public decimal? SnowTemperature { get; set; }
    
    /// <summary>Ilmankosteus (%)</summary>
    public decimal? Humidity { get; set; }
    
    /// <summary>Lumen laatu/kunto</summary>
    public string? SnowCondition { get; set; }
    
    /// <summary>Lisämuistiinpanot</summary>
    public string? Notes { get; set; }
    
    /// <summary>Luontiaika</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>Suksiparit mukana tässä testissä</summary>
    public ICollection<TestEventPair> TestEventPairs { get; set; } = new List<TestEventPair>();
    
    /// <summary>Ottelut tähän testiin liittyen</summary>
    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
