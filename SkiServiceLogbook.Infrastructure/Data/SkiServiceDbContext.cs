using Microsoft.EntityFrameworkCore;
using SkiServiceLogbook.Domain.Entities;
using SkiServiceLogbook.Domain.Enums;

namespace SkiServiceLogbook.Infrastructure.Data;

/// <summary>
/// Tietokantakonteksti EF Core:lle
/// </summary>
public class SkiServiceDbContext : DbContext
{
    public SkiServiceDbContext(DbContextOptions<SkiServiceDbContext> options) : base(options) { }

    // DbSet-määrittelyt
    public DbSet<User> Users => Set<User>();
    public DbSet<SkiPair> SkiPairs => Set<SkiPair>();
    public DbSet<Ski> Skis => Set<Ski>();
    public DbSet<WaxCategory> WaxCategories => Set<WaxCategory>();
    public DbSet<Wax> Waxes => Set<Wax>();
    public DbSet<TestEvent> TestEvents => Set<TestEvent>();
    public DbSet<TestEventPair> TestEventPairs => Set<TestEventPair>();
    public DbSet<TestEventPairWax> TestEventPairWaxes => Set<TestEventPairWax>();
    public DbSet<WaxInstruction> WaxInstructions => Set<WaxInstruction>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User - käyttäjä
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Role).HasConversion<int>();
            
            // Indeksit
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // SkiPair - suksipari
        modelBuilder.Entity<SkiPair>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Brand).HasMaxLength(100);
            entity.Property(e => e.Model).HasMaxLength(100);
            
            // Indeksi
            entity.HasIndex(e => e.PairNumber).IsUnique();
        });

        // Ski - yksittäinen suksi
        modelBuilder.Entity<Ski>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Side).IsRequired().HasMaxLength(10);
            entity.Property(e => e.SerialNumber).HasMaxLength(100);
            
            // Suhde SkiPair:iin (cascade delete)
            entity.HasOne(e => e.SkiPair)
                .WithMany(sp => sp.Skis)
                .HasForeignKey(e => e.SkiPairId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indeksi
            entity.HasIndex(e => new { e.SkiPairId, e.Side });
        });

        // WaxCategory - voitekategoria
        modelBuilder.Entity<WaxCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            
            // Indeksi
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Wax - voide
        modelBuilder.Entity<Wax>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.MinTemperature).HasPrecision(5, 2);
            entity.Property(e => e.MaxTemperature).HasPrecision(5, 2);
            entity.Property(e => e.MinHumidity).HasPrecision(5, 2);
            entity.Property(e => e.MaxHumidity).HasPrecision(5, 2);
            entity.Property(e => e.SnowCondition).HasMaxLength(255);
            
            // Suhde WaxCategory:yn (restrict delete)
            entity.HasOne(e => e.WaxCategory)
                .WithMany(wc => wc.Waxes)
                .HasForeignKey(e => e.WaxCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Indeksi
            entity.HasIndex(e => new { e.WaxCategoryId, e.Name });
        });

        // TestEvent - testitapahtuma
        modelBuilder.Entity<TestEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.AirTemperature).HasPrecision(5, 2);
            entity.Property(e => e.SnowTemperature).HasPrecision(5, 2);
            entity.Property(e => e.Humidity).HasPrecision(5, 2);
            entity.Property(e => e.SnowCondition).HasMaxLength(255);
            
            // Indeksi olosuhteille
            entity.HasIndex(e => new { e.AirTemperature, e.Humidity });
            entity.HasIndex(e => e.EventDate);
        });

        // TestEventPair - testitulos suksiparille
        modelBuilder.Entity<TestEventPair>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MeasuredResult).HasPrecision(10, 3);
            
            // Suhteet
            entity.HasOne(e => e.TestEvent)
                .WithMany(te => te.TestEventPairs)
                .HasForeignKey(e => e.TestEventId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.SkiPair)
                .WithMany(sp => sp.TestEventPairs)
                .HasForeignKey(e => e.SkiPairId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Indeksi
            entity.HasIndex(e => new { e.TestEventId, e.SkiPairId });
        });

        // TestEventPairWax - voide testissä
        modelBuilder.Entity<TestEventPairWax>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // Suhteet
            entity.HasOne(e => e.TestEventPair)
                .WithMany(tep => tep.TestEventPairWaxes)
                .HasForeignKey(e => e.TestEventPairId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Wax)
                .WithMany(w => w.TestEventPairWaxes)
                .HasForeignKey(e => e.WaxId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Indeksi
            entity.HasIndex(e => new { e.TestEventPairId, e.LayerOrder });
        });

        // WaxInstruction - voiteluohje
        modelBuilder.Entity<WaxInstruction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Instructions).IsRequired();
        });

        // Match - ottelu
        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TournamentName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<int>();
            
            // Suhteet
            entity.HasOne(e => e.TestEvent)
                .WithMany(te => te.Matches)
                .HasForeignKey(e => e.TestEventId)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(e => e.SkiPair1)
                .WithMany(sp => sp.MatchesAsSkiPair1)
                .HasForeignKey(e => e.SkiPair1Id)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.SkiPair2)
                .WithMany(sp => sp.MatchesAsSkiPair2)
                .HasForeignKey(e => e.SkiPair2Id)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.WinnerSkiPair)
                .WithMany(sp => sp.MatchesAsWinner)
                .HasForeignKey(e => e.WinnerSkiPairId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Indeksit
            entity.HasIndex(e => new { e.TournamentName, e.Round, e.MatchNumber });
        });

        // AuditLog - muutosloki
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Action).HasConversion<int>();
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            
            // Suhde User:iin
            entity.HasOne(e => e.User)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Indeksit
            entity.HasIndex(e => new { e.EntityType, e.EntityId });
            entity.HasIndex(e => e.Timestamp);
        });
    }
}
