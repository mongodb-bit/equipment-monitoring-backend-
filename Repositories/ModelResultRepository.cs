using EquipmentMonitoringAPI.Data;
using EquipmentMonitoringAPI.DTOs;
using EquipmentMonitoringAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquipmentMonitoringAPI.Repositories;

public class ModelResultRepository : IModelResultRepository
{
    private readonly EquipmentDbContext _context;

    public ModelResultRepository(EquipmentDbContext context)
    {
        _context = context;
    }

    // Get latest anomalous result for each equipment
    public async Task<List<ModelResultDTO>> GetFaultyEquipmentAsync()
    {
        var results = await _context.ModelResults
            .Where(x => x.IsAnomaly)
            .Select(x => new ModelResultDTO
            {
                Id = x.Id,

                EquipmentId = x.EquipmentId,

                TelemetryId = x.TelemetryId,

                IsAnomaly = x.IsAnomaly,

                AnomalyScore = x.AnomalyScore,

                RiskLevel = x.RiskLevel,

                CreatedAt = x.CreatedAt,

                EquipmentName = x.Equipment.Name,

                Temperature = x.Telemetry.Temperature,

                Pressure = x.Telemetry.Pressure,

                FlowRate = x.Telemetry.FlowRate,

                RuntimeHours = x.Telemetry.RuntimeHours,

                ErrorCount = x.Telemetry.ErrorCount
            })
            .ToListAsync();

        // Get only the latest anomaly for each equipment
        return results
            .GroupBy(x => x.EquipmentId)
            .Select(g => g
                .OrderByDescending(x => x.CreatedAt)
                .First())
            .ToList();
    }


    // Get latest model result for one equipment
    public async Task<ModelResultDTO?> GetLatestByEquipmentIdAsync(
        int equipmentId)
    {
        return await _context.ModelResults
            .Where(x => x.EquipmentId == equipmentId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ModelResultDTO
            {
                Id = x.Id,

                EquipmentId = x.EquipmentId,

                TelemetryId = x.TelemetryId,

                IsAnomaly = x.IsAnomaly,

                AnomalyScore = x.AnomalyScore,

                RiskLevel = x.RiskLevel,

                CreatedAt = x.CreatedAt,

                EquipmentName = x.Equipment.Name,

                Temperature = x.Telemetry.Temperature,

                Pressure = x.Telemetry.Pressure,

                FlowRate = x.Telemetry.FlowRate,

                RuntimeHours = x.Telemetry.RuntimeHours,

                ErrorCount = x.Telemetry.ErrorCount
            })
            .FirstOrDefaultAsync();
    }
}