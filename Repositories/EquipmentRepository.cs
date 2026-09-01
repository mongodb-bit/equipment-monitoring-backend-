using EquipmentMonitoringAPI.Data;
using EquipmentMonitoringAPI.Models;
using EquipmentMonitoringAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquipmentMonitoringAPI.Repositories;

public class EquipmentRepository : IEquipmentRepository
{
    private readonly EquipmentDbContext _context;

    public EquipmentRepository(EquipmentDbContext context)
    {
        _context = context;
    }

    public async Task<List<Equipment>> GetAllAsync()
    {
        return await _context.Equipment
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Equipment?> GetByIdAsync(int id)
    {
        return await _context.Equipment
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Equipment> CreateAsync(Equipment equipment)
    {
        _context.Equipment.Add(equipment);

        await _context.SaveChangesAsync();

        return equipment;
    }

    public async Task UpdateAsync(Equipment equipment)
    {
        _context.Equipment.Update(equipment);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Equipment equipment)
    {
        _context.Equipment.Remove(equipment);

        await _context.SaveChangesAsync();
    }
}