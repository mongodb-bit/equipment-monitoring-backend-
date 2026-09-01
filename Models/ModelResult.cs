namespace EquipmentMonitoringAPI.Models;

public class ModelResult
{
    public int Id { get; set; }

    // Equipment that was analyzed
    public int EquipmentId { get; set; }

    // Telemetry that was sent to ML
    public int TelemetryId { get; set; }

    // ML prediction
    public bool IsAnomaly { get; set; }

    public double AnomalyScore { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public string ModelVersion { get; set; } = "v1";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Equipment Equipment { get; set; } = null!;

    public Telemetry Telemetry { get; set; } = null!;
}