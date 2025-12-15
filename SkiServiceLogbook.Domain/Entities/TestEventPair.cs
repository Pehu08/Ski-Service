namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Yksittäisen suksipar in testitulos tietyssä testissä
/// </summary>
public class TestEventPair
{
    /// <summary>Testituloksen yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Viittaus testiin</summary>
    public int TestEventId { get; set; }
    
    /// <summary>Viittaus suksipariin</summary>
    public int SkiPairId { get; set; }
    
    /// <summary>Sijoitus testissä (1 = paras)</summary>
    public int? Ranking { get; set; }
    
    /// <summary>Testin mittaustulos (esim. aika sekunteina)</summary>
    public decimal? MeasuredResult { get; set; }
    
    /// <summary>Muistiinpanot tuloksesta</summary>
    public string? Notes { get; set; }
    
    /// <summary>Navigaatio testiin</summary>
    public TestEvent TestEvent { get; set; } = null!;
    
    /// <summary>Navigaatio suksipariin</summary>
    public SkiPair SkiPair { get; set; } = null!;
    
    /// <summary>Käytetyt voiteet tässä testissä</summary>
    public ICollection<TestEventPairWax> TestEventPairWaxes { get; set; } = new List<TestEventPairWax>();
}
