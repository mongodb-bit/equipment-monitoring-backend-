using EquipmentMonitoringAPI.Models;
using EquipmentMonitoringAPI.Repositories.Interfaces;

namespace EquipmentMonitoringAPI.Services;

public class TelemetryService
{
    private readonly ITelemetryRepository _repository;

    public TelemetryService(
        ITelemetryRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Telemetry>> GetByEquipmentIdAsync(
        int equipmentId)
    {
        return await _repository
            .GetByEquipmentIdAsync(equipmentId);
    }
}