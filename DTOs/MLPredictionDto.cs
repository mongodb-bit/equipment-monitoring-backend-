namespace EquipmentMonitoringAPI.DTOs;

public class MLPredictionRequestDto
{
    public int EquipmentId { get; set; }

    public double Temperature { get; set; }

    public double Pressure { get; set; }

    public double FlowRate { get; set; }

    public double RuntimeHours { get; set; }

    public int ErrorCount { get; set; }

    public double MinTemperature { get; set; }

    public double MaxTemperature { get; set; }

    public double MinPressure { get; set; }

    public double MaxPressure { get; set; }

    public double MinFlowRate { get; set; }

    public double MaxFlowRate { get; set; }

    public int MaxErrorCount { get; set; }
}

public class MLPredictionResponseDto
{
    public int EquipmentId { get; set; }

    public bool IsAnomaly { get; set; }

    public double AnomalyScore { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public string ModelVersion { get; set; } = string.Empty;
}