namespace EquipmentMonitoringAPI.DTOs;

public class CreateEquipmentDto
{
    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public double MinTemperature { get; set; }

    public double MaxTemperature { get; set; }

    public double MinPressure { get; set; }

    public double MaxPressure { get; set; }

    public double MinFlowRate { get; set; }

    public double MaxFlowRate { get; set; }

    public int MaxErrorCount { get; set; }
}


public class UpdateEquipmentDto
{
    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Status { get; set; } = "Active";

    public double MinTemperature { get; set; }

    public double MaxTemperature { get; set; }

    public double MinPressure { get; set; }

    public double MaxPressure { get; set; }

    public double MinFlowRate { get; set; }

    public double MaxFlowRate { get; set; }

    public int MaxErrorCount { get; set; }
}


public class EquipmentResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public double MinTemperature { get; set; }

    public double MaxTemperature { get; set; }

    public double MinPressure { get; set; }

    public double MaxPressure { get; set; }

    public double MinFlowRate { get; set; }

    public double MaxFlowRate { get; set; }

    public int MaxErrorCount { get; set; }

    public DateTime CreatedAt { get; set; }
}