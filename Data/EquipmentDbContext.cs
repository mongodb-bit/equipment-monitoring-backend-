using EquipmentMonitoringAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentMonitoringAPI.Data;

public class EquipmentDbContext : DbContext
{
    public EquipmentDbContext(
        DbContextOptions<EquipmentDbContext> options)
        : base(options)
    {
    }

    public DbSet<Equipment> Equipment { get; set; }

    public DbSet<Telemetry> Telemetry { get; set; }

    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }

    public DbSet<ModelResult> ModelResults { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Equipment -> Telemetry
        modelBuilder.Entity<Telemetry>()
            .HasOne(t => t.Equipment)
            .WithMany(e => e.Telemetries)
            .HasForeignKey(t => t.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Equipment -> ModelResult
        modelBuilder.Entity<ModelResult>()
            .HasOne(m => m.Equipment)
            .WithMany(e => e.ModelResults)
            .HasForeignKey(m => m.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Telemetry -> ModelResult
        modelBuilder.Entity<ModelResult>()
            .HasOne(m => m.Telemetry)
            .WithMany()
            .HasForeignKey(m => m.TelemetryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Equipment -> MaintenanceRecord
        modelBuilder.Entity<MaintenanceRecord>()
            .HasOne(m => m.Equipment)
            .WithMany(e => e.MaintenanceRecords)
            .HasForeignKey(m => m.EquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> MaintenanceRecord
        modelBuilder.Entity<MaintenanceRecord>()
            .HasOne(m => m.PerformedByUser)
            .WithMany(u => u.MaintenanceRecords)
            .HasForeignKey(m => m.PerformedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // User configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}