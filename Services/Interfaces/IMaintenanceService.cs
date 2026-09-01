using EquipmentMonitoringAPI.DTOs;

namespace EquipmentMonitoringAPI.Services.Interfaces;

public interface IMaintenanceService
{
    Task<MaintenanceRecordResponseDto> CreateAsync(
        CreateMaintenanceRecordDto dto,
        int userId);

    Task<List<MaintenanceRecordResponseDto>> GetAllAsync();

    Task<List<MaintenanceRecordResponseDto>> GetByEquipmentIdAsync(
        int equipmentId);
}