using EquipmentMonitoringAPI.Models;

namespace EquipmentMonitoringAPI.Repositories.Interfaces;

public interface ITelemetryRepository
{
    Task CreateAsync(Telemetry telemetry);

    Task<List<Telemetry>> GetByEquipmentIdAsync(int equipmentId);

    Task<Telemetry?> GetLatestByEquipmentIdAsync(int equipmentId);
}