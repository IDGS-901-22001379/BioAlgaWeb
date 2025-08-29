using System.Threading.Tasks;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IVentaService
    {
        // EXISTENTES
        Task<VentaDto> RegistrarVentaAsync(int idUsuario, VentaCreateRequest req);
        Task<bool> CancelarVentaAsync(int idUsuario, int idVenta);

        // NUEVOS: Historial y Detalle
        Task<PagedResponse<VentaResumenDto>> BuscarVentasAsync(VentaQueryParams qp);
        Task<VentaDetalleDto?> ObtenerVentaPorIdAsync(int idVenta);
    }
}
