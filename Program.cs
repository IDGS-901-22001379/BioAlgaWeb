using System.Text.Json.Serialization;
using BioAlga.Backend.Data;
using BioAlga.Backend.Mapping;
using BioAlga.Backend.Repositories;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
        // Evita ciclos al serializar (por relaciones Rol/Empleado)
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
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
// AutoMapper
// ===============================
builder.Services.AddAutoMapper(typeof(UsuarioProfile).Assembly);

// ===============================
// Inyección de dependencias (Usuarios)
// ===============================
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

// ===============================
// CORS (Angular)
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontCors", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // URL de tu Angular
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ===============================
// Pipeline
// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar CORS para Angular
app.UseCors("FrontCors");

app.UseAuthorization();

app.MapControllers();

app.Run();
