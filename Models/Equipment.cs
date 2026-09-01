namespace EquipmentMonitoringAPI.Models;

public class Equipment
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Status { get; set; } = "Active";

    // Temperature thresholds
    public double MinTemperature { get; set; }

    public double MaxTemperature { get; set; }

    // Pressure thresholds
    public double MinPressure { get; set; }

    public double MaxPressure { get; set; }

    // Flow rate thresholds
    public double MinFlowRate { get; set; }

    public double MaxFlowRate { get; set; }

    // Maximum allowed errors
    public int MaxErrorCount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public ICollection<Telemetry> Telemetries { get; set; }
        = new List<Telemetry>();

    public ICollection<MaintenanceRecord> MaintenanceRecords { get; set; }
        = new List<MaintenanceRecord>();

    public ICollection<ModelResult> ModelResults { get; set; }
        = new List<ModelResult>();
}