using System.Text;

using EquipmentMonitoringAPI.Data;
using EquipmentMonitoringAPI.Helpers;
using EquipmentMonitoringAPI.Repositories;
using EquipmentMonitoringAPI.Repositories.Interfaces;
using EquipmentMonitoringAPI.Services;
using EquipmentMonitoringAPI.Services.Interfaces;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// Controllers
// ==================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();


// ==================================================
// Swagger + JWT Authentication
// ==================================================

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",

            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,

            Scheme = "Bearer",

            BearerFormat = "JWT",

            In = Microsoft.OpenApi.Models.ParameterLocation.Header,

            Description =
                "Enter your JWT token. Example: Bearer {your token}"
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,

                            Id = "Bearer"
                        }
                },

                Array.Empty<string>()
            }
        });
});


// ==================================================
// Entity Framework Core
// ==================================================

builder.Services.AddDbContext<EquipmentDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);


// ==================================================
// Repositories
// ==================================================

// Equipment
builder.Services.AddScoped<
    IEquipmentRepository,
    EquipmentRepository
>();

// Maintenance
builder.Services.AddScoped<
    IMaintenanceRepository,
    MaintenanceRepository
>();

// Telemetry
builder.Services.AddScoped<
    ITelemetryRepository,
    TelemetryRepository
>();

// Model Result
builder.Services.AddScoped<
    IModelResultRepository,
    ModelResultRepository
>();


// ==================================================
// Services
// ==================================================

// Equipment
builder.Services.AddScoped<
    IEquipmentService,
    EquipmentService
>();

// Maintenance
builder.Services.AddScoped<
    IMaintenanceService,
    MaintenanceService
>();

// Model Result
builder.Services.AddScoped<ModelResultService>();

// Authentication Service
builder.Services.AddScoped<AuthService>();

// JWT Helper
builder.Services.AddScoped<JwtHelper>();


// ==================================================
// HTTP CLIENT FOR PYTHON ML API
// ==================================================

// TelemetryGeneratorService uses this client
// to communicate with the Python FastAPI ML server.

builder.Services.AddHttpClient("MLClient", client =>
{
    client.BaseAddress =
        new Uri("http://127.0.0.1:8000");

    client.Timeout =
        TimeSpan.FromSeconds(5);
});


// ==================================================
// TELEMETRY GENERATOR
// ==================================================

// Runs continuously in the background.
//
// Every 10 seconds:
//
// 1. Get all ACTIVE equipment
// 2. Generate telemetry
// 3. Save telemetry
// 4. Send telemetry to Python ML API
// 5. Receive prediction
// 6. Save ModelResult

builder.Services.AddHostedService<TelemetryGeneratorService>();


// ==================================================
// JWT CONFIGURATION
// ==================================================

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException(
        "JWT Key is not configured."
    );
}


// ==================================================
// AUTHENTICATION
// ==================================================

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ValidIssuer =
                builder.Configuration["Jwt:Issuer"],

            ValidAudience =
                builder.Configuration["Jwt:Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)
                ),

            ClockSkew = TimeSpan.Zero
        };
});


// ==================================================
// AUTHORIZATION
// ==================================================

builder.Services.AddAuthorization();

// Telemetry
builder.Services.AddScoped<TelemetryService>();
// ==================================================
// BUILD APPLICATION
// ==================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactUI", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});




var app = builder.Build();


// ==================================================
// HTTP REQUEST PIPELINE
// ==================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// IMPORTANT:
// Authentication MUST come before Authorization

app.UseAuthentication();

app.UseAuthorization();
app.UseCors("AllowReactUI");

// ==================================================
// CONTROLLERS
// ==================================================

app.MapControllers();


// ==================================================
// RUN
// ==================================================

app.Run();