using SkiServiceLogbook.Domain.Enums;

namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Järjestelmän muutosloki
/// </summary>
public class AuditLog
{
    /// <summary>Lokin yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Viittaus käyttäjään joka teki toiminnon</summary>
    public int UserId { get; set; }
    
    /// <summary>Toiminnon tyyppi</summary>
    public AuditAction Action { get; set; }
    
    /// <summary>Kohde-entityn tyyppi (esim. "SkiPair")</summary>
    public string EntityType { get; set; } = string.Empty;
    
    /// <summary>Kohde-entityn ID</summary>
    public int? EntityId { get; set; }
    
    /// <summary>Vanhat arvot JSON-muodossa</summary>
    public string? OldValues { get; set; }
    
    /// <summary>Uudet arvot JSON-muodossa</summary>
    public string? NewValues { get; set; }
    
    /// <summary>IP-osoite</summary>
    public string? IpAddress { get; set; }
    
    /// <summary>Aikaleima</summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>Navigaatio käyttäjään</summary>
    public User User { get; set; } = null!;
}
