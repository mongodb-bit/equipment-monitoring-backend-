using EquipmentMonitoringAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentMonitoringAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ModelResultController : ControllerBase
{
    private readonly ModelResultService _service;

    public ModelResultController(
        ModelResultService service)
    {
        _service = service;
    }

    // GET: api/modelresult/faulty
    // ADMIN + OPERATOR + MAINTENANCE_ENGINEER
    [HttpGet("faulty")]
    [Authorize(
        Roles = "ADMIN,OPERATOR,MAINTENANCE_ENGINEER")]
    public async Task<IActionResult> GetFaultyEquipment()
    {
        var results =
            await _service.GetFaultyEquipmentAsync();

        return Ok(results);
    }

    // GET: api/modelresult/equipment/1
    // Get latest ML result for one equipment
    [HttpGet("equipment/{equipmentId:int}")]
    [Authorize(
        Roles = "ADMIN,OPERATOR,MAINTENANCE_ENGINEER")]
    public async Task<IActionResult> GetLatestResult(
        int equipmentId)
    {
        var result =
            await _service.GetLatestByEquipmentIdAsync(
                equipmentId);

        if (result == null)
        {
            return NotFound(new
            {
                message =
                    $"No model result found for Equipment ID {equipmentId}."
            });
        }

        return Ok(result);
    }
}