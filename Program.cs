using System.Text.Json.Serialization;
using BioAlga.Backend.Data;

// Repos & Services
using BioAlga.Backend.Repositories;
using BioAlga.Backend.Repositories.Interfaces;
using BioAlga.Backend.Services;
using BioAlga.Backend.Services.Interfaces;

// AutoMapper Profiles
using BioAlga.Backend.Mapping;

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
        opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); // enums como texto
        // opt.JsonSerializerOptions.PropertyNamingPolicy = null;
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
// AutoMapper (carga todos los Profiles)
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

// ======== Ventas / Devoluciones / Caja ========
builder.Services.AddScoped<IVentaRepository, VentaRepository>();
builder.Services.AddScoped<IVentaService, VentaService>();

// DEVOLUCIONES (NUEVO)
builder.Services.AddScoped<IDevolucionRepository, DevolucionRepository>(); // <—
builder.Services.AddScoped<IDevolucionService, DevolucionService>();       // <—

builder.Services.AddScoped<ICajaService, CajaService>(); // si ya lo tienes

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

// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
