using System.Net.Http.Json;
using EquipmentMonitoringAPI.DTOs;
using EquipmentMonitoringAPI.Models;

namespace EquipmentMonitoringAPI.Services;

public class MLModelService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MLModelService> _logger;

    public MLModelService(
        HttpClient httpClient,
        ILogger<MLModelService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<MLPredictionResponseDto?> PredictAsync(
        Equipment equipment,
        Telemetry telemetry)
    {
        var request = new MLPredictionRequestDto
        {
            EquipmentId = equipment.Id,

            Temperature = telemetry.Temperature,
            Pressure = telemetry.Pressure,
            FlowRate = telemetry.FlowRate,
            RuntimeHours = telemetry.RuntimeHours,
            ErrorCount = telemetry.ErrorCount,

            MinTemperature = equipment.MinTemperature,
            MaxTemperature = equipment.MaxTemperature,

            MinPressure = equipment.MinPressure,
            MaxPressure = equipment.MaxPressure,

            MinFlowRate = equipment.MinFlowRate,
            MaxFlowRate = equipment.MaxFlowRate,

            MaxErrorCount = equipment.MaxErrorCount
        };

        try
        {
            _logger.LogInformation(
                "Sending telemetry for Equipment {EquipmentId} to ML model.",
                equipment.Id
            );

            var response = await _httpClient.PostAsJsonAsync(
                "/predict",
                request
            );

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "ML API returned status code {StatusCode} for Equipment {EquipmentId}.",
                    response.StatusCode,
                    equipment.Id
                );

                return null;
            }

            var result =
                await response.Content
                    .ReadFromJsonAsync<MLPredictionResponseDto>();

            if (result != null)
            {
                _logger.LogInformation(
                    "ML prediction received for Equipment {EquipmentId}: " +
                    "Anomaly={IsAnomaly}, Score={Score}, Risk={RiskLevel}",
                    result.EquipmentId,
                    result.IsAnomaly,
                    result.AnomalyScore,
                    result.RiskLevel
                );
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error calling ML API for Equipment {EquipmentId}.",
                equipment.Id
            );

            return null;
        }
    }
}