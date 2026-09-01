namespace EquipmentMonitoringAPI.Models;

public class MaintenanceRecord
{
    public int Id { get; set; }

    // Equipment being maintained
    public int EquipmentId { get; set; }

    // User who performed the maintenance
    public int PerformedByUserId { get; set; }

    public string MaintenanceType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime MaintenanceDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Equipment Equipment { get; set; } = null!;

    public User PerformedByUser { get; set; } = null!;
}