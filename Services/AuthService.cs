using EquipmentMonitoringAPI.Data;
using EquipmentMonitoringAPI.DTOs;
using EquipmentMonitoringAPI.Helpers;
using EquipmentMonitoringAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EquipmentMonitoringAPI.Services;

public class AuthService
{
    private readonly EquipmentDbContext _context;
    private readonly JwtHelper _jwtHelper;

    public AuthService(
        EquipmentDbContext context,
        JwtHelper jwtHelper)
    {
        _context = context;
        _jwtHelper = jwtHelper;
    }

    // --------------------------------------------------
    // Register
    // --------------------------------------------------

    public async Task<string> RegisterAsync(RegisterDto dto)
    {
        // Validate role
        var allowedRoles = new[]
        {
            "ADMIN",
            "OPERATOR",
            "MAINTENANCE_ENGINEER"
        };

        if (!allowedRoles.Contains(dto.Role.ToUpper()))
        {
            throw new ArgumentException(
                "Invalid role. Allowed roles are ADMIN, OPERATOR, MAINTENANCE_ENGINEER."
            );
        }

        // Check username
        var usernameExists = await _context.Users
            .AnyAsync(u => u.Username == dto.Username);

        if (usernameExists)
        {
            throw new ArgumentException(
                "Username already exists."
            );
        }

        // Check email
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == dto.Email);

        if (emailExists)
        {
            throw new ArgumentException(
                "Email already exists."
            );
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(
            dto.Password
        );

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Role = dto.Role.ToUpper(),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return "User registered successfully.";
    }

    // --------------------------------------------------
    // Login
    // --------------------------------------------------

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.Username == dto.Username);

        if (user == null)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password."
            );
        }

        // Verify password
        var passwordValid =
            BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash
            );

        if (!passwordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password."
            );
        }

        // Generate JWT
        return _jwtHelper.GenerateToken(user);
    }
}