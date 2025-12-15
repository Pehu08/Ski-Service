namespace SkiServiceLogbook.Domain.Entities;

/// <summary>
/// Yksittäinen voide lämpötila- ja kosteusalueineen
/// </summary>
public class Wax
{
    /// <summary>Voideen yksilöivä tunniste</summary>
    public int Id { get; set; }
    
    /// <summary>Viittaus voideluainekategoriaan</summary>
    public int WaxCategoryId { get; set; }
    
    /// <summary>Voideen nimi (esim. "LF Blue")</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Valmistaja (esim. "Swix", "Toko")</summary>
    public string Manufacturer { get; set; } = string.Empty;
    
    /// <summary>Minimäärä lämpötila (Celsius)</summary>
    public decimal? MinTemperature { get; set; }
    
    /// <summary>Maksimilämpötila (Celsius)</summary>
    public decimal? MaxTemperature { get; set; }
    
    /// <summary>Minimikosteus (%)</summary>
    public decimal? MinHumidity { get; set; }
    
    /// <summary>Maksimikosteus (%)</summary>
    public decimal? MaxHumidity { get; set; }
    
    /// <summary>Lumiolosuhteiden kuvaus</summary>
    public string? SnowCondition { get; set; }
    
    /// <summary>Onko voide käytössä (soft delete)</summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>Navigaatio kategoriaan</summary>
    public WaxCategory WaxCategory { get; set; } = null!;
    
    /// <summary>Testit joissa tätä voidetta on käytetty</summary>
    public ICollection<TestEventPairWax> TestEventPairWaxes { get; set; } = new List<TestEventPairWax>();
}
