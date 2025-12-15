using SkiServiceLogbook.Domain.Entities;
using SkiServiceLogbook.Domain.Enums;

namespace SkiServiceLogbook.Infrastructure.Data;

/// <summary>
/// Siemendatan alustus tietokantaan
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Alustaa tietokannan siemendatalla
    /// </summary>
    public static async Task InitializeAsync(SkiServiceDbContext context)
    {
        // Tarkista onko dataa jo olemassa
        if (context.SkiPairs.Any())
        {
            return;
        }

        // 1. Luo voitekategoriat
        var categories = new[]
        {
            new WaxCategory { Name = "Luistovoide", Description = "Glide wax - käytetään liukupinnalle", DisplayOrder = 1 },
            new WaxCategory { Name = "Pitovoide", Description = "Grip wax - käytetään pitopinnalle", DisplayOrder = 2 },
            new WaxCategory { Name = "Pinnoite", Description = "Top coating - viimeinen kerros", DisplayOrder = 3 }
        };
        context.WaxCategories.AddRange(categories);
        await context.SaveChangesAsync();

        // 2. Luo voiteet lämpötila-alueineen
        var waxes = new[]
        {
            // Luistovoiteet
            new Wax { WaxCategoryId = 1, Name = "LF Blue", Manufacturer = "Swix", MinTemperature = -8, MaxTemperature = -2, SnowCondition = "Uusi lumi", IsActive = true },
            new Wax { WaxCategoryId = 1, Name = "LF Red", Manufacturer = "Swix", MinTemperature = -2, MaxTemperature = 2, SnowCondition = "Kostea lumi", IsActive = true },
            new Wax { WaxCategoryId = 1, Name = "CH7 Blue", Manufacturer = "Toko", MinTemperature = -12, MaxTemperature = -6, SnowCondition = "Kylmä karkea lumi", IsActive = true },
            new Wax { WaxCategoryId = 1, Name = "HF Yellow", Manufacturer = "Rex", MinTemperature = 0, MaxTemperature = 5, SnowCondition = "Märkä lumi", IsActive = true },
            
            // Pitovoiteet
            new Wax { WaxCategoryId = 2, Name = "V30 Blue", Manufacturer = "Swix", MinTemperature = -7, MaxTemperature = -1, SnowCondition = "Kylmä kova lumi", IsActive = true },
            new Wax { WaxCategoryId = 2, Name = "V40 Blue Extra", Manufacturer = "Swix", MinTemperature = -10, MaxTemperature = -3, SnowCondition = "Uusi lumi", IsActive = true },
            new Wax { WaxCategoryId = 2, Name = "V50 Violet", Manufacturer = "Swix", MinTemperature = -4, MaxTemperature = 0, SnowCondition = "Muuttuva lumi", IsActive = true },
            
            // Pinnoitteet
            new Wax { WaxCategoryId = 3, Name = "FC8X", Manufacturer = "Swix", MinTemperature = -8, MaxTemperature = 0, SnowCondition = "Kilpailut, kylmä", IsActive = true },
            new Wax { WaxCategoryId = 3, Name = "FC7X", Manufacturer = "Swix", MinTemperature = -2, MaxTemperature = 4, SnowCondition = "Kilpailut, lauha", IsActive = true },
            new Wax { WaxCategoryId = 3, Name = "HelX 2.0 Yellow", Manufacturer = "Toko", MinTemperature = 0, MaxTemperature = 10, SnowCondition = "Lämmin märkä", IsActive = true }
        };
        context.Waxes.AddRange(waxes);
        await context.SaveChangesAsync();

        // 3. Luo 10 suksiparia (1-10, a/b)
        var skiPairs = new List<SkiPair>();
        var brands = new[] { "Fischer", "Atomic", "Salomon", "Rossignol", "Madshus" };
        var models = new[] { "Speedmax", "Redster", "S/Race", "Delta", "Nanosonic" };
        
        for (int i = 1; i <= 10; i++)
        {
            var skiPair = new SkiPair
            {
                PairNumber = i,
                Brand = brands[(i - 1) % brands.Length],
                Model = models[(i - 1) % models.Length],
                Length = 190 + (i % 3) * 2, // 190, 192, 194 cm
                IsActive = true,
                Notes = $"Suksipari {i}",
                Skis = new List<Ski>
                {
                    new Ski { Side = "a", SerialNumber = $"SN{i:D3}A" },
                    new Ski { Side = "b", SerialNumber = $"SN{i:D3}B" }
                }
            };
            skiPairs.Add(skiPair);
        }
        context.SkiPairs.AddRange(skiPairs);
        await context.SaveChangesAsync();

        // 4. Luo esimerkkikäyttäjä (Admin)
        var adminUser = new User
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            FullName = "Admin Käyttäjä",
            Email = "admin@skiservice.fi",
            Role = Role.Admin,
            IsActive = true
        };
        context.Users.Add(adminUser);
        await context.SaveChangesAsync();

        // 5. Luo esimerkki testitapahtuma
        var testEvent = new TestEvent
        {
            Name = "Vuokatti Testit 2024",
            EventDate = new DateTime(2024, 12, 15),
            Location = "Vuokatti",
            AirTemperature = -5,
            SnowTemperature = -6,
            Humidity = 75,
            SnowCondition = "Kova uusi lumi",
            Notes = "Aamupäivän testit"
        };
        context.TestEvents.Add(testEvent);
        await context.SaveChangesAsync();

        // 6. Lisää 5 ensimmäistä paria testiin
        var testEventPairs = new List<TestEventPair>();
        for (int i = 1; i <= 5; i++)
        {
            var pair = new TestEventPair
            {
                TestEventId = testEvent.Id,
                SkiPairId = i,
                Ranking = i,
                MeasuredResult = 120.5m + (i * 0.3m), // Simuloidut ajat
                Notes = $"Testi {i}"
            };
            testEventPairs.Add(pair);
        }
        context.TestEventPairs.AddRange(testEventPairs);
        await context.SaveChangesAsync();

        // 7. Lisää voitelut parille 1 (paras tulos)
        var waxCombination = new[]
        {
            new TestEventPairWax { TestEventPairId = testEventPairs[0].Id, WaxId = 1, LayerOrder = 1 }, // LF Blue base
            new TestEventPairWax { TestEventPairId = testEventPairs[0].Id, WaxId = 8, LayerOrder = 2 }  // FC8X top
        };
        context.TestEventPairWaxes.AddRange(waxCombination);
        await context.SaveChangesAsync();

        // 8. Luo esimerkki voiteluohje
        var instruction = new WaxInstruction
        {
            Title = "Kylmän kelin voitelu",
            Instructions = "1. Puhdista pohjat\n2. Levitä LF Blue -pohjavoide\n3. Harjaa huolellisesti\n4. Lisää FC8X-pinnoite\n5. Kiillota",
            Conditions = "Ilman lämpötila -5 ... -10°C, kova lumi"
        };
        context.WaxInstructions.Add(instruction);
        await context.SaveChangesAsync();
    }
}
