using EquipmentMonitoringAPI.DTOs;
using EquipmentMonitoringAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentMonitoringAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _service;

    public EquipmentController(IEquipmentService service)
    {
        _service = service;
    }

    // GET: api/equipment
    // ADMIN + OPERATOR + MAINTENANCE_ENGINEER
    [HttpGet]
    [Authorize(Roles = "ADMIN,OPERATOR,MAINTENANCE_ENGINEER")]
    public async Task<IActionResult> GetAll()
    {
        var equipment = await _service.GetAllAsync();

        return Ok(equipment);
    }

    // GET: api/equipment/1
    // ADMIN + OPERATOR + MAINTENANCE_ENGINEER
    [HttpGet("{id:int}")]
    [Authorize(Roles = "ADMIN,OPERATOR,MAINTENANCE_ENGINEER")]
    public async Task<IActionResult> GetById(int id)
    {
        var equipment = await _service.GetByIdAsync(id);

        if (equipment == null)
        {
            return NotFound(new
            {
                message = $"Equipment with ID {id} not found."
            });
        }

        return Ok(equipment);
    }

    // POST: api/equipment
    // ADMIN ONLY
    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create(
        [FromBody] CreateEquipmentDto dto)
    {
        try
        {
            var equipment = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = equipment.Id },
                equipment
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // PUT: api/equipment/1
    // ADMIN ONLY
    [HttpPut("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateEquipmentDto dto)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto);

            if (!updated)
            {
                return NotFound(new
                {
                    message = $"Equipment with ID {id} not found."
                });
            }

            return Ok(new
            {
                message = "Equipment updated successfully."
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    // DELETE: api/equipment/1
    // ADMIN ONLY
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Equipment with ID {id} not found."
            });
        }

        return Ok(new
        {
            message = "Equipment deleted successfully."
        });
    }
}