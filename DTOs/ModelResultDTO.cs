namespace EquipmentMonitoringAPI.DTOs;

public class ModelResultDTO
{
    public int Id { get; set; }

    public int EquipmentId { get; set; }

    public int TelemetryId { get; set; }

    public bool IsAnomaly { get; set; }

    public double AnomalyScore { get; set; }

    public string RiskLevel { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    // Equipment
    public string? EquipmentName { get; set; }

    // Telemetry
    public double Temperature { get; set; }

    public double Pressure { get; set; }

    public double FlowRate { get; set; }

    public double RuntimeHours { get; set; }

    public int ErrorCount { get; set; }
}