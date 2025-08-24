using System.Text.Json.Serialization;
using BioAlga.Backend.Data;
using BioAlga.Backend.Repositories;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using BioAlga.Backend.Mapping; // Para detectar MappingProfiles (Cliente, Empleado, etc.)

var builder = WebApplication.CreateBuilder(args);

// ===============================
// Base de Datos (MySQL - Pomelo)
// ===============================
var connectionString = builder.Configuration.GetConnectionString("MySql")
    ?? throw new InvalidOperationException("Falta ConnectionStrings:MySql en appsettings.json");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ===============================
// Controllers / JSON
// ===============================
builder.Services
    .AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        // opt.JsonSerializerOptions.PropertyNamingPolicy = null; // Descomenta si quieres PascalCase en JSON
    });

// ===============================
// Swagger
// ===============================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BioAlga API",
        Version = "v1",
        Description = "API BioAlga"
    });
});

// ===============================
// AutoMapper (carga todos los Profiles del ensamblado)
// ===============================
builder.Services.AddAutoMapper(typeof(ClienteMappingProfile).Assembly);

// ===============================
// Inyección de dependencias
// ===============================
// Usuarios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// Clientes
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();

// Empleados
builder.Services.AddScoped<IEmpleadoRepository, EmpleadoRepository>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();

// >>> Proveedores (NUEVO)
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();

// ===============================
// CORS (Angular)
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontCors", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "http://127.0.0.1:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// ===============================
// Pipeline HTTP
// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontCors");

// (Si agregas auth en el futuro, coloca app.UseAuthentication() aquí)
app.UseAuthorization();

app.MapControllers();
app.Run();
