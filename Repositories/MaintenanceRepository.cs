using EquipmentMonitoringAPI.Data;
using EquipmentMonitoringAPI.Models;
using EquipmentMonitoringAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquipmentMonitoringAPI.Repositories;

public class MaintenanceRepository : IMaintenanceRepository
{
    private readonly EquipmentDbContext _context;

    public MaintenanceRepository(EquipmentDbContext context)
    {
        _context = context;
    }

    public async Task<MaintenanceRecord> CreateAsync(
        MaintenanceRecord record)
    {
        _context.MaintenanceRecords.Add(record);

        await _context.SaveChangesAsync();

        return record;
    }

    public async Task<List<MaintenanceRecord>> GetByEquipmentIdAsync(
        int equipmentId)
    {
        return await _context.MaintenanceRecords
            .Include(m => m.Equipment)
            .Include(m => m.PerformedByUser)
            .Where(m => m.EquipmentId == equipmentId)
            .OrderByDescending(m => m.MaintenanceDate)
            .ToListAsync();
    }

    public async Task<List<MaintenanceRecord>> GetAllAsync()
    {
        return await _context.MaintenanceRecords
            .Include(m => m.Equipment)
            .Include(m => m.PerformedByUser)
            .OrderByDescending(m => m.MaintenanceDate)
            .ToListAsync();
    }
}