using EquipmentMonitoringAPI.DTOs;
using EquipmentMonitoringAPI.Models;
using EquipmentMonitoringAPI.Repositories.Interfaces;
using EquipmentMonitoringAPI.Services.Interfaces;

namespace EquipmentMonitoringAPI.Services;

public class EquipmentService : IEquipmentService
{
    private readonly IEquipmentRepository _repository;

    public EquipmentService(IEquipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EquipmentResponseDto>> GetAllAsync()
    {
        var equipment = await _repository.GetAllAsync();

        return equipment.Select(MapToDto).ToList();
    }

    public async Task<EquipmentResponseDto?> GetByIdAsync(int id)
    {
        var equipment = await _repository.GetByIdAsync(id);

        if (equipment == null)
            return null;

        return MapToDto(equipment);
    }

    public async Task<EquipmentResponseDto> CreateAsync(
        CreateEquipmentDto dto)
    {
        if (dto.MaxTemperature <= dto.MinTemperature)
            throw new ArgumentException(
                "Maximum temperature must be greater than minimum temperature.");

        if (dto.MaxPressure <= dto.MinPressure)
            throw new ArgumentException(
                "Maximum pressure must be greater than minimum pressure.");

        if (dto.MaxFlowRate <= dto.MinFlowRate)
            throw new ArgumentException(
                "Maximum flow rate must be greater than minimum flow rate.");

        if (dto.MaxErrorCount < 0)
            throw new ArgumentException(
                "Maximum error count cannot be negative.");

        var equipment = new Equipment
        {
            Name = dto.Name,
            Type = dto.Type,
            Location = dto.Location,

            Status = "Active",

            MinTemperature = dto.MinTemperature,
            MaxTemperature = dto.MaxTemperature,

            MinPressure = dto.MinPressure,
            MaxPressure = dto.MaxPressure,

            MinFlowRate = dto.MinFlowRate,
            MaxFlowRate = dto.MaxFlowRate,

            MaxErrorCount = dto.MaxErrorCount,

            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(equipment);

        return MapToDto(created);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateEquipmentDto dto)
    {
        var equipment = await _repository.GetByIdAsync(id);

        if (equipment == null)
            return false;

        if (dto.MaxTemperature <= dto.MinTemperature)
            throw new ArgumentException(
                "Maximum temperature must be greater than minimum temperature.");

        if (dto.MaxPressure <= dto.MinPressure)
            throw new ArgumentException(
                "Maximum pressure must be greater than minimum pressure.");

        if (dto.MaxFlowRate <= dto.MinFlowRate)
            throw new ArgumentException(
                "Maximum flow rate must be greater than minimum flow rate.");

        equipment.Name = dto.Name;
        equipment.Type = dto.Type;
        equipment.Location = dto.Location;
        equipment.Status = dto.Status;

        equipment.MinTemperature = dto.MinTemperature;
        equipment.MaxTemperature = dto.MaxTemperature;

        equipment.MinPressure = dto.MinPressure;
        equipment.MaxPressure = dto.MaxPressure;

        equipment.MinFlowRate = dto.MinFlowRate;
        equipment.MaxFlowRate = dto.MaxFlowRate;

        equipment.MaxErrorCount = dto.MaxErrorCount;

        await _repository.UpdateAsync(equipment);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var equipment = await _repository.GetByIdAsync(id);

        if (equipment == null)
            return false;

        await _repository.DeleteAsync(equipment);

        return true;
    }

    private static EquipmentResponseDto MapToDto(
        Equipment equipment)
    {
        return new EquipmentResponseDto
        {
            Id = equipment.Id,
            Name = equipment.Name,
            Type = equipment.Type,
            Location = equipment.Location,
            Status = equipment.Status,

            MinTemperature = equipment.MinTemperature,
            MaxTemperature = equipment.MaxTemperature,

            MinPressure = equipment.MinPressure,
            MaxPressure = equipment.MaxPressure,

            MinFlowRate = equipment.MinFlowRate,
            MaxFlowRate = equipment.MaxFlowRate,

            MaxErrorCount = equipment.MaxErrorCount,

            CreatedAt = equipment.CreatedAt
        };
    }
}