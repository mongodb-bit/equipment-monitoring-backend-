using EquipmentMonitoringAPI.Models;

namespace EquipmentMonitoringAPI.Repositories.Interfaces;

public interface IMaintenanceRepository
{
    Task<MaintenanceRecord> CreateAsync(MaintenanceRecord record);

    Task<List<MaintenanceRecord>> GetByEquipmentIdAsync(int equipmentId);

    Task<List<MaintenanceRecord>> GetAllAsync();
}