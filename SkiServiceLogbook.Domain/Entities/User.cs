using SkiServiceLogbook.Domain.Enums;

namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Järjestelmän käyttäjä
/// </summary>
public class User
{
    /// <summary>Käyttäjän yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Käyttäjätunnus kirjautumista varten</summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>Salasanan hash (bcrypt)</summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>Käyttäjän koko nimi</summary>
    public string FullName { get; set; } = string.Empty;
    
    /// <summary>Sähköpostiosoite</summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>Käyttäjän rooli järjestelmässä</summary>
    public Role Role { get; set; } = Role.Seuraaja;
    
    /// <summary>Onko käyttäjätili aktiivinen</summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>Tilin luontiaika</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>Viimeisin kirjautumisaika</summary>
    public DateTime? LastLoginAt { get; set; }
    
    /// <summary>Käyttäjän tekemät toiminnot audit logissa</summary>
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
