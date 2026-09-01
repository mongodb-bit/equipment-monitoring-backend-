namespace EquipmentMonitoringAPI.DTOs;

public class CreateMaintenanceRecordDto
{
    public int EquipmentId { get; set; }

    public string MaintenanceType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime MaintenanceDate { get; set; } = DateTime.UtcNow;
}

public class MaintenanceRecordResponseDto
{
    public int Id { get; set; }

    public int EquipmentId { get; set; }

    public string EquipmentName { get; set; } = string.Empty;

    public int PerformedByUserId { get; set; }

    public string PerformedByUsername { get; set; } = string.Empty;

    public string MaintenanceType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime MaintenanceDate { get; set; }

    public DateTime CreatedAt { get; set; }
}