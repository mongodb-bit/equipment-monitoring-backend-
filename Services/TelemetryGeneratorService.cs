using System.Net.Http.Json;
using System.Text.Json.Serialization;
using EquipmentMonitoringAPI.Data;
using EquipmentMonitoringAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentMonitoringAPI.Services;

public class TelemetryGeneratorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TelemetryGeneratorService> _logger;

    private readonly Random _random = new Random();

    public TelemetryGeneratorService(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<TelemetryGeneratorService> logger)
    {
        _scopeFactory = scopeFactory;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    // =========================================================
    // Background service
    // =========================================================

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Telemetry Generator Service started."
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await GenerateTelemetryAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while generating telemetry."
                );
            }

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(10),
                    stoppingToken
                );
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation(
            "Telemetry Generator Service stopped."
        );
    }

    // =========================================================
    // Generate telemetry for all active equipment
    // =========================================================

    private async Task GenerateTelemetryAsync(
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var context =
            scope.ServiceProvider
                .GetRequiredService<EquipmentDbContext>();

        // Get all ACTIVE equipment
        var equipmentList =
            await context.Equipment
                .Where(e => e.Status == "Active")
                .ToListAsync(cancellationToken);

        if (equipmentList.Count == 0)
        {
            _logger.LogInformation(
                "No active equipment found."
            );

            return;
        }

        foreach (var equipment in equipmentList)
        {
            try
            {
                // =================================================
                // 1. Generate telemetry
                // =================================================

                var telemetry =
                    GenerateForEquipment(equipment);

                context.Telemetry.Add(telemetry);

                // Save first so Telemetry.Id is generated
                await context.SaveChangesAsync(
                    cancellationToken
                );

                _logger.LogInformation(
                    "Telemetry saved. EquipmentId={EquipmentId}, TelemetryId={TelemetryId}",
                    equipment.Id,
                    telemetry.Id
                );

                // =================================================
                // 2. Immediately send telemetry to Python ML API
                // =================================================

                var mlResult =
                    await CallMlModelAsync(
                        telemetry,
                        equipment,
                        cancellationToken
                    );

                if (mlResult == null)
                {
                    _logger.LogWarning(
                        "ML model did not return a result for TelemetryId={TelemetryId}",
                        telemetry.Id
                    );

                    continue;
                }

                // =================================================
                // 3. Save ML result
                // =================================================

                var modelResult = new ModelResult
                {
                    EquipmentId = equipment.Id,

                    TelemetryId = telemetry.Id,

                    IsAnomaly = mlResult.IsAnomaly,

                    AnomalyScore = mlResult.AnomalyScore,

                    RiskLevel = mlResult.RiskLevel,

                    ModelVersion = mlResult.ModelVersion,

                    CreatedAt = DateTime.UtcNow
                };

                context.ModelResults.Add(modelResult);

                await context.SaveChangesAsync(
                    cancellationToken
                );

                // =================================================
                // 4. Log result
                // =================================================

                if (mlResult.IsAnomaly)
                {
                    _logger.LogWarning(
                        "ANOMALY DETECTED! EquipmentId={EquipmentId}, " +
                        "TelemetryId={TelemetryId}, Risk={RiskLevel}, Score={Score}",
                        equipment.Id,
                        telemetry.Id,
                        mlResult.RiskLevel,
                        mlResult.AnomalyScore
                    );
                }
                else
                {
                    _logger.LogInformation(
                        "ML Result: EquipmentId={EquipmentId}, " +
                        "Risk={RiskLevel}, Score={Score}",
                        equipment.Id,
                        mlResult.RiskLevel,
                        mlResult.AnomalyScore
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing telemetry for EquipmentId={EquipmentId}",
                    equipment.Id
                );
            }
        }
    }

    // =========================================================
    // Call Python ML API
    // =========================================================

    private async Task<MLResultDto?> CallMlModelAsync(
        Telemetry telemetry,
        Equipment equipment,
        CancellationToken cancellationToken)
    {
        // Use named HTTP client
        var client =
            _httpClientFactory.CreateClient("MLClient");

        // =====================================================
        // IMPORTANT:
        // Send BOTH telemetry values AND equipment thresholds
        // =====================================================

        var request = new MLRequestDto
        {
            EquipmentId = equipment.Id,

            // Telemetry
            Temperature = telemetry.Temperature,
            Pressure = telemetry.Pressure,
            FlowRate = telemetry.FlowRate,
            RuntimeHours = telemetry.RuntimeHours,
            ErrorCount = telemetry.ErrorCount,

            // Equipment thresholds
            MinTemperature = equipment.MinTemperature,
            MaxTemperature = equipment.MaxTemperature,

            MinPressure = equipment.MinPressure,
            MaxPressure = equipment.MaxPressure,

            MinFlowRate = equipment.MinFlowRate,
            MaxFlowRate = equipment.MaxFlowRate,

            MaxErrorCount = equipment.MaxErrorCount
        };

        _logger.LogInformation(
            "Sending TelemetryId={TelemetryId} to ML API...",
            telemetry.Id
        );

        try
        {
            var response =
                await client.PostAsJsonAsync(
                    "/predict",
                    request,
                    cancellationToken
                );

            // =================================================
            // Handle Python errors
            // =================================================

            if (!response.IsSuccessStatusCode)
            {
                var error =
                    await response.Content.ReadAsStringAsync(
                        cancellationToken
                    );

                _logger.LogError(
                    "ML API returned {StatusCode}: {Error}",
                    response.StatusCode,
                    error
                );

                return null;
            }

            // =================================================
            // Read prediction
            // =================================================

            var result =
                await response.Content
                    .ReadFromJsonAsync<MLResultDto>(
                        cancellationToken
                    );

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Could not connect to ML API."
            );

            return null;
        }
    }

    // =========================================================
    // Generate telemetry values
    // =========================================================

    private Telemetry GenerateForEquipment(
        Equipment equipment)
    {
        // 15% chance of generating abnormal telemetry
        bool abnormal =
            _random.NextDouble() < 0.15;

        double temperature;
        double pressure;
        double flowRate;
        int errorCount;

        if (!abnormal)
        {
            // =================================================
            // NORMAL VALUES
            // =================================================

            temperature =
                RandomDouble(
                    equipment.MinTemperature,
                    equipment.MaxTemperature
                );

            pressure =
                RandomDouble(
                    equipment.MinPressure,
                    equipment.MaxPressure
                );

            flowRate =
                RandomDouble(
                    equipment.MinFlowRate,
                    equipment.MaxFlowRate
                );

            errorCount =
                _random.Next(
                    0,
                    equipment.MaxErrorCount + 1
                );
        }
        else
        {
            // =================================================
            // ABNORMAL VALUES
            // =================================================

            temperature =
                GenerateAbnormalValue(
                    equipment.MinTemperature,
                    equipment.MaxTemperature
                );

            pressure =
                GenerateAbnormalValue(
                    equipment.MinPressure,
                    equipment.MaxPressure
                );

            flowRate =
                GenerateAbnormalValue(
                    equipment.MinFlowRate,
                    equipment.MaxFlowRate
                );

            errorCount =
                equipment.MaxErrorCount
                + _random.Next(1, 6);
        }

        return new Telemetry
        {
            EquipmentId = equipment.Id,

            Temperature =
                Math.Round(
                    temperature,
                    2
                ),

            Pressure =
                Math.Round(
                    pressure,
                    2
                ),

            FlowRate =
                Math.Round(
                    flowRate,
                    2
                ),

            RuntimeHours =
                Math.Round(
                    _random.NextDouble() * 24,
                    2
                ),

            ErrorCount = errorCount,

            Timestamp = DateTime.UtcNow
        };
    }

    // =========================================================
    // Random value between min and max
    // =========================================================

    private double RandomDouble(
        double min,
        double max)
    {
        return min +
               (_random.NextDouble()
               * (max - min));
    }

    // =========================================================
    // Generate abnormal value
    // =========================================================

    private double GenerateAbnormalValue(
        double min,
        double max)
    {
        double range = max - min;

        bool above =
            _random.Next(0, 2) == 0;

        if (above)
        {
            return max +
                   (_random.NextDouble()
                   * range * 0.5);
        }

        return min -
               (_random.NextDouble()
               * range * 0.5);
    }
}


// ==========================================================
// DTO SENT TO PYTHON ML API
// ==========================================================

public class MLRequestDto
{
    [JsonPropertyName("equipmentId")]
    public int EquipmentId { get; set; }

    // -----------------------------
    // Telemetry
    // -----------------------------

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("pressure")]
    public double Pressure { get; set; }

    [JsonPropertyName("flowRate")]
    public double FlowRate { get; set; }

    [JsonPropertyName("runtimeHours")]
    public double RuntimeHours { get; set; }

    [JsonPropertyName("errorCount")]
    public int ErrorCount { get; set; }

    // -----------------------------
    // Equipment thresholds
    // -----------------------------

    [JsonPropertyName("minTemperature")]
    public double MinTemperature { get; set; }

    [JsonPropertyName("maxTemperature")]
    public double MaxTemperature { get; set; }

    [JsonPropertyName("minPressure")]
    public double MinPressure { get; set; }

    [JsonPropertyName("maxPressure")]
    public double MaxPressure { get; set; }

    [JsonPropertyName("minFlowRate")]
    public double MinFlowRate { get; set; }

    [JsonPropertyName("maxFlowRate")]
    public double MaxFlowRate { get; set; }

    [JsonPropertyName("maxErrorCount")]
    public int MaxErrorCount { get; set; }
}


// ==========================================================
// DTO RECEIVED FROM PYTHON ML API
// ==========================================================

public class MLResultDto
{
    public int EquipmentId { get; set; }

    public bool IsAnomaly { get; set; }

    public double AnomalyScore { get; set; }

    public string RiskLevel { get; set; }
        = string.Empty;

    public string ModelVersion { get; set; }
        = "v1";
}