using EquipmentMonitoringAPI.DTOs;

namespace EquipmentMonitoringAPI.Services.Interfaces;

public interface IEquipmentService
{
    Task<List<EquipmentResponseDto>> GetAllAsync();

    Task<EquipmentResponseDto?> GetByIdAsync(int id);

    Task<EquipmentResponseDto> CreateAsync(CreateEquipmentDto dto);

    Task<bool> UpdateAsync(int id, UpdateEquipmentDto dto);

    Task<bool> DeleteAsync(int id);
}