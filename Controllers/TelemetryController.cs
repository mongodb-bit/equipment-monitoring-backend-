using EquipmentMonitoringAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentMonitoringAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryService _service;

    public TelemetryController(
        TelemetryService service)
    {
        _service = service;
    }

    // GET:
    // api/telemetry/equipment/1
    //
    // ADMIN + OPERATOR + MAINTENANCE_ENGINEER
    [HttpGet("equipment/{equipmentId:int}")]
    [Authorize(
        Roles = "ADMIN,OPERATOR,MAINTENANCE_ENGINEER")]
    public async Task<IActionResult> GetByEquipmentId(
        int equipmentId)
    {
        var telemetry =
            await _service.GetByEquipmentIdAsync(
                equipmentId);

        if (telemetry.Count == 0)
        {
            return NotFound(new
            {
                message =
                    $"No telemetry found for Equipment ID {equipmentId}."
            });
        }

        return Ok(telemetry);
    }
}