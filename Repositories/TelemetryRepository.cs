using EquipmentMonitoringAPI.Data;
using EquipmentMonitoringAPI.Models;
using EquipmentMonitoringAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquipmentMonitoringAPI.Repositories;

public class TelemetryRepository : ITelemetryRepository
{
    private readonly EquipmentDbContext _context;

    public TelemetryRepository(EquipmentDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Telemetry telemetry)
    {
        _context.Telemetry.Add(telemetry);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Telemetry>> GetByEquipmentIdAsync(
        int equipmentId)
    {
        return await _context.Telemetry
            .Where(t => t.EquipmentId == equipmentId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();
    }

    public async Task<Telemetry?> GetLatestByEquipmentIdAsync(
        int equipmentId)
    {
        return await _context.Telemetry
            .Where(t => t.EquipmentId == equipmentId)
            .OrderByDescending(t => t.Timestamp)
            .FirstOrDefaultAsync();
    }
}