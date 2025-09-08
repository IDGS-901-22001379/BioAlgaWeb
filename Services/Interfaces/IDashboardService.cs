using BioAlga.Backend.Dtos.Dashboard;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<IEnumerable<VentasResumenDto>> GetVentasResumenAsync();
        Task<IEnumerable<TopProductoDto>> GetTopProductosAsync();
        Task<IEnumerable<TopClienteDto>> GetTopClientesAsync();
        Task<IEnumerable<VentasPorUsuarioDto>> GetVentasPorUsuariosAsync();
        Task<IEnumerable<DevolucionesPorUsuarioDto>> GetDevolucionesPorUsuariosAsync();
        Task<IEnumerable<ComprasPorProveedorDto>> GetComprasPorProveedoresAsync();
    }
}
