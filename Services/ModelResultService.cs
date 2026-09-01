using EquipmentMonitoringAPI.DTOs;
using EquipmentMonitoringAPI.Repositories.Interfaces;

namespace EquipmentMonitoringAPI.Services;

public class ModelResultService
{
    private readonly IModelResultRepository _repository;

    public ModelResultService(
        IModelResultRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ModelResultDTO>> GetFaultyEquipmentAsync()
    {
        return await _repository.GetFaultyEquipmentAsync();
    }

    public async Task<ModelResultDTO?> GetLatestByEquipmentIdAsync(
        int equipmentId)
    {
        return await _repository
            .GetLatestByEquipmentIdAsync(equipmentId);
    }
}