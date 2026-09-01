using EquipmentMonitoringAPI.DTOs;
using EquipmentMonitoringAPI.Models;
using EquipmentMonitoringAPI.Repositories.Interfaces;
using EquipmentMonitoringAPI.Services.Interfaces;

namespace EquipmentMonitoringAPI.Services;

public class MaintenanceService : IMaintenanceService
{
    private readonly IMaintenanceRepository _maintenanceRepository;

    public MaintenanceService(
        IMaintenanceRepository maintenanceRepository)
    {
        _maintenanceRepository = maintenanceRepository;
    }

    public async Task<MaintenanceRecordResponseDto> CreateAsync(
        CreateMaintenanceRecordDto dto,
        int userId)
    {
        if (string.IsNullOrWhiteSpace(dto.MaintenanceType))
            throw new ArgumentException(
                "Maintenance type is required.");

        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException(
                "Maintenance description is required.");

        var record = new MaintenanceRecord
        {
            EquipmentId = dto.EquipmentId,
            PerformedByUserId = userId,
            MaintenanceType = dto.MaintenanceType,
            Description = dto.Description,
            MaintenanceDate = dto.MaintenanceDate
        };

        var created = await _maintenanceRepository
            .CreateAsync(record);

        return new MaintenanceRecordResponseDto
        {
            Id = created.Id,
            EquipmentId = created.EquipmentId,
            PerformedByUserId = created.PerformedByUserId,
            MaintenanceType = created.MaintenanceType,
            Description = created.Description,
            MaintenanceDate = created.MaintenanceDate,
            CreatedAt = created.CreatedAt
        };
    }

    public async Task<List<MaintenanceRecordResponseDto>> GetAllAsync()
    {
        var records = await _maintenanceRepository.GetAllAsync();

        return records.Select(record => new MaintenanceRecordResponseDto
        {
            Id = record.Id,
            EquipmentId = record.EquipmentId,
            EquipmentName = record.Equipment.Name,
            PerformedByUserId = record.PerformedByUserId,
            PerformedByUsername = record.PerformedByUser.Username,
            MaintenanceType = record.MaintenanceType,
            Description = record.Description,
            MaintenanceDate = record.MaintenanceDate,
            CreatedAt = record.CreatedAt
        }).ToList();
    }

    public async Task<List<MaintenanceRecordResponseDto>>
        GetByEquipmentIdAsync(int equipmentId)
    {
        var records = await _maintenanceRepository
            .GetByEquipmentIdAsync(equipmentId);

        return records.Select(record => new MaintenanceRecordResponseDto
        {
            Id = record.Id,
            EquipmentId = record.EquipmentId,
            EquipmentName = record.Equipment.Name,
            PerformedByUserId = record.PerformedByUserId,
            PerformedByUsername = record.PerformedByUser.Username,
            MaintenanceType = record.MaintenanceType,
            Description = record.Description,
            MaintenanceDate = record.MaintenanceDate,
            CreatedAt = record.CreatedAt
        }).ToList();
    }
}