using System.Text.Json.Serialization;
using BioAlga.Backend.Data;
using BioAlga.Backend.Repositories;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using BioAlga.Backend.Mapping; // Para detectar MappingProfiles (Cliente, Empleado, Venta, etc.)

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
        // Enums como texto ("Entrada","Salida","Efectivo","Pagada", etc.)
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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

// Proveedores
builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<IProveedorService, ProveedorService>();

// Productos & Precios
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IPrecioRepository, PrecioRepository>();
builder.Services.AddScoped<IPrecioService, PrecioService>();

// Compras + Inventario
builder.Services.AddScoped<ICompraRepository, CompraRepository>();
builder.Services.AddScoped<ICompraService, CompraService>();
builder.Services.AddScoped<IInventarioRepository, InventarioRepository>();
// (Si luego agregas un servicio de inventario de más alto nivel, regístralo aquí)

// ======== Ventas / Devoluciones / Caja ========
builder.Services.AddScoped<IVentaRepository, VentaRepository>();   // <-- NUEVO (repositorio de ventas)
builder.Services.AddScoped<IVentaService, VentaService>();         // servicio de ventas

builder.Services.AddScoped<IDevolucionService, DevolucionService>(); // si ya lo tienes implementado
builder.Services.AddScoped<ICajaService, CajaService>();             // si ya lo tienes implementado

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

// (Si agregas auth en el futuro)
/// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
