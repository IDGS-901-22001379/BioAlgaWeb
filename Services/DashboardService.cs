using AutoMapper;
using AutoMapper.QueryableExtensions;
using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos.Dashboard;
using BioAlga.Backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DashboardService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ======= Ventas resumen (día, semana, mes, año) =======
        public async Task<IEnumerable<VentasResumenDto>> GetVentasResumenAsync()
        {
            return await _context.VentasResumen
                .AsNoTracking()
                .ProjectTo<VentasResumenDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // ======= Top productos por ingreso/unidades =======
        public async Task<IEnumerable<TopProductoDto>> GetTopProductosAsync()
        {
            return await _context.TopProductos
                .AsNoTracking()
                .ProjectTo<TopProductoDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // ======= Top clientes =======
        public async Task<IEnumerable<TopClienteDto>> GetTopClientesAsync()
        {
            return await _context.TopClientes
                .AsNoTracking()
                .ProjectTo<TopClienteDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // ======= Ventas por usuario =======
        public async Task<IEnumerable<VentasPorUsuarioDto>> GetVentasPorUsuariosAsync()
        {
            return await _context.VentasPorUsuarios
                .AsNoTracking()
                .ProjectTo<VentasPorUsuarioDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // ======= Devoluciones por usuario =======
        public async Task<IEnumerable<DevolucionesPorUsuarioDto>> GetDevolucionesPorUsuariosAsync()
        {
            return await _context.DevolucionesPorUsuarios
                .AsNoTracking()
                .ProjectTo<DevolucionesPorUsuarioDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        // ======= Compras por proveedor =======
        public async Task<IEnumerable<ComprasPorProveedorDto>> GetComprasPorProveedoresAsync()
        {
            return await _context.ComprasPorProveedores
                .AsNoTracking()
                .ProjectTo<ComprasPorProveedorDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}
