using EquipmentMonitoringAPI.Models;

namespace EquipmentMonitoringAPI.Repositories.Interfaces;

public interface IEquipmentRepository
{
    Task<List<Equipment>> GetAllAsync();

    Task<Equipment?> GetByIdAsync(int id);

    Task<Equipment> CreateAsync(Equipment equipment);

    Task UpdateAsync(Equipment equipment);

    Task DeleteAsync(Equipment equipment);
}