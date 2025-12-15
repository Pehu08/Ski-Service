namespace SkiServiceLogbook.Domain.Enums;

/// <summary>
/// Audit log -toiminnon tyyppi
/// </summary>
public enum AuditAction
{
    /// <summary>Luotu - uusi tietue luotu</summary>
    Created = 0,
    
    /// <summary>Päivitetty - olemassa oleva tietue muokattu</summary>
    Updated = 1,
    
    /// <summary>Poistettu - tietue poistettu</summary>
    Deleted = 2,
    
    /// <summary>Kirjauduttu - käyttäjä kirjautui sisään</summary>
    Login = 3,
    
    /// <summary>Kirjauduttu ulos - käyttäjä kirjautui ulos</summary>
    Logout = 4
}
