namespace SkiServiceLogbook.Domain.Enums;

/// <summary>
/// Ottelun tila turnausbracketeissa
/// </summary>
public enum MatchStatus
{
    /// <summary>Odottaa - ottelu ei ole vielä alkanut</summary>
    Pending = 0,
    
    /// <summary>Käynnissä - ottelu on menossa</summary>
    InProgress = 1,
    
    /// <summary>Valmis - ottelu on päättynyt</summary>
    Completed = 2,
    
    /// <summary>Bye - vastustajaa ei ole, automaattinen voitto</summary>
    Bye = 3
}
