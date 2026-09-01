using EquipmentMonitoringAPI.DTOs;

namespace EquipmentMonitoringAPI.Repositories.Interfaces;

public interface IModelResultRepository
{
    Task<List<ModelResultDTO>> GetFaultyEquipmentAsync();

    Task<ModelResultDTO?> GetLatestByEquipmentIdAsync(
        int equipmentId);
}