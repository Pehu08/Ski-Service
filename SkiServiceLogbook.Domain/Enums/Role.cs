namespace SkiServiceLogbook.Domain.Enums;

/// <summary>
/// Käyttäjän rooli järjestelmässä
/// </summary>
public enum Role
{
    /// <summary>Seuraaja - vain lukuoikeus raportteihin</summary>
    Seuraaja = 0,
    
    /// <summary>Urheilija - voi katsella omia tuloksia ja testejä</summary>
    Urheilija = 1,
    
    /// <summary>Huolto - voi lisätä testejä, tuloksia ja voiteluja</summary>
    Huolto = 2,
    
    /// <summary>Admin - täydet oikeudet, varaston hallinta</summary>
    Admin = 3
}
