using System.Threading.Tasks;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IVentaService
    {
        Task<VentaDto> RegistrarVentaAsync(int idUsuario, VentaCreateRequest req);
        Task<bool> CancelarVentaAsync(int idUsuario, int idVenta);
    }
}
