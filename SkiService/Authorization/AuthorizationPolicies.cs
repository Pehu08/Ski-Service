namespace SkiServiceLogbook.Authorization;

/// <summary>
/// Valtuutuskäytäntöjen nimet
/// </summary>
public static class AuthorizationPolicies
{
    /// <summary>Varaston hallinta - Admin</summary>
    public const string CanManageInventory = "CanManageInventory";
    
    /// <summary>Testien luonti - Admin, Huolto</summary>
    public const string CanCreateTests = "CanCreateTests";
    
    /// <summary>Tulosten syöttö - Admin, Huolto, Urheilija</summary>
    public const string CanEnterResults = "CanEnterResults";
    
    /// <summary>Raporttien lukuoikeus - kaikki roolit</summary>
    public const string ReadOnlyReports = "ReadOnlyReports";
}
