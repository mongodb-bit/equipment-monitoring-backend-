namespace EquipmentMonitoringAPI.Models;

public class Telemetry
{
    public int Id { get; set; }

    // Foreign key
    public int EquipmentId { get; set; }

    public double Temperature { get; set; }

    public double Pressure { get; set; }

    public double FlowRate { get; set; }

    public double RuntimeHours { get; set; }

    public int ErrorCount { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Equipment Equipment { get; set; } = null!;
}