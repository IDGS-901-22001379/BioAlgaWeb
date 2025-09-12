using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos.Dashboard;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======= Ventas resumen (día, semana, mes, año) =======
        public async Task<IEnumerable<VentasResumenDto>> GetVentasResumenAsync()
        {
            return await _context.VentasResumen
                .AsNoTracking()
                .Select(v => new VentasResumenDto
                {
                    Dia = v.Dia,
                    Anio = v.Anio,
                    Mes = v.Mes,
                    Semana = v.Semana,
                    TotalVentas = v.TotalVentas,
                    Subtotal = v.Subtotal,
                    Impuestos = v.Impuestos,
                    NumTickets = v.NumTickets
                })
                .ToListAsync();
        }

        // ======= Top productos por ingreso/unidades =======
        public async Task<IEnumerable<TopProductoDto>> GetTopProductosAsync()
        {
            return await _context.TopProductos
                .AsNoTracking()
                .Select(p => new TopProductoDto
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    TotalUnidades = p.TotalUnidades,
                    IngresoTotal = p.IngresoTotal
                })
                .ToListAsync();
        }

        // ======= Top clientes =======
        public async Task<IEnumerable<TopClienteDto>> GetTopClientesAsync()
        {
            return await _context.TopClientes
                .AsNoTracking()
                .Select(c => new TopClienteDto
                {
                    IdCliente = c.IdCliente,
                    NombreCompleto = c.NombreCompleto,
                    TotalGastado = c.TotalGastado
                })
                .ToListAsync();
        }

        // ======= Ventas por usuario =======
        public async Task<IEnumerable<VentasPorUsuarioDto>> GetVentasPorUsuariosAsync()
        {
            return await _context.VentasPorUsuarios
                .AsNoTracking()
                .Select(u => new VentasPorUsuarioDto
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    ApellidoPaterno = u.ApellidoPaterno,
                    TotalVendido = u.TotalVendido,
                    NumVentas = u.NumVentas
                })
                .ToListAsync();
        }

        // ======= Devoluciones por usuario =======
        public async Task<IEnumerable<DevolucionesPorUsuarioDto>> GetDevolucionesPorUsuariosAsync()
        {
            return await _context.DevolucionesPorUsuarios
                .AsNoTracking()
                .Select(d => new DevolucionesPorUsuarioDto
                {
                    IdUsuario = d.IdUsuario,
                    NombreUsuario = d.NombreUsuario,
                    NumDevoluciones = d.NumDevoluciones,
                    TotalDevuelto = d.TotalDevuelto
                })
                .ToListAsync();
        }

        // ======= Compras por proveedor =======
        public async Task<IEnumerable<ComprasPorProveedorDto>> GetComprasPorProveedoresAsync()
        {
            return await _context.ComprasPorProveedores
                .AsNoTracking()
                .Select(p => new ComprasPorProveedorDto
                {
                    IdProveedor = p.IdProveedor,
                    NombreEmpresa = p.NombreEmpresa,
                    TotalComprado = p.TotalComprado,
                    NumCompras = p.NumCompras
                })
                .ToListAsync();
        }
    }
}
