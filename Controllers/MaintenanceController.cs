using System.Security.Claims;
using EquipmentMonitoringAPI.DTOs;
using EquipmentMonitoringAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentMonitoringAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaintenanceController : ControllerBase
{
    private readonly IMaintenanceService _service;

    public MaintenanceController(IMaintenanceService service)
    {
        _service = service;
    }

    // GET: api/maintenance
    // ADMIN + OPERATOR + MAINTENANCE_ENGINEER
    [HttpGet]
    [Authorize(Roles = "ADMIN,OPERATOR,MAINTENANCE_ENGINEER")]
    public async Task<IActionResult> GetAll()
    {
        var records = await _service.GetAllAsync();

        return Ok(records);
    }

    // GET: api/maintenance/equipment/1
    // ADMIN + OPERATOR + MAINTENANCE_ENGINEER
    [HttpGet("equipment/{equipmentId:int}")]
    [Authorize(Roles = "ADMIN,OPERATOR,MAINTENANCE_ENGINEER")]
    public async Task<IActionResult> GetByEquipmentId(int equipmentId)
    {
        var records = await _service.GetByEquipmentIdAsync(equipmentId);

        return Ok(records);
    }

    // POST: api/maintenance
    // MAINTENANCE_ENGINEER ONLY
    [HttpPost]
    [Authorize(Roles = "MAINTENANCE_ENGINEER")]
    public async Task<IActionResult> Create(
        [FromBody] CreateMaintenanceRecordDto dto)
    {
        try
        {
            // Get UserId from JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized(new
                {
                    message = "User ID not found in token."
                });
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid user ID in token."
                });
            }

            var record = await _service.CreateAsync(dto, userId);

            return Ok(record);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
}