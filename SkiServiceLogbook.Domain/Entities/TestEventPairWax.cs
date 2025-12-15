namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Voide joka on käytetty suksipar ille tietyssä testissä
/// </summary>
public class TestEventPairWax
{
    /// <summary>Yhdistelmän yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Viittaus testitulokseen</summary>
    public int TestEventPairId { get; set; }
    
    /// <summary>Viittaus voiteeseen</summary>
    public int WaxId { get; set; }
    
    /// <summary>Kerrosjärjestys (1 = alin kerros)</summary>
    public int LayerOrder { get; set; }
    
    /// <summary>Navigaatio testitulokseen</summary>
    public TestEventPair TestEventPair { get; set; } = null!;
    
    /// <summary>Navigaatio voiteeseen</summary>
    public Wax Wax { get; set; } = null!;
}
