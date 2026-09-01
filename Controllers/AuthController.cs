using EquipmentMonitoringAPI.DTOs;
using EquipmentMonitoringAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EquipmentMonitoringAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    // POST: api/auth/register
    // ONLY ADMIN can register users
    [HttpPost("register")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);

            return Ok(new
            {
                message = result
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

    // POST: api/auth/login
    // Everyone can login without authentication
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var token = await _authService.LoginAsync(dto);

            return Ok(new
            {
                token = token
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }
}