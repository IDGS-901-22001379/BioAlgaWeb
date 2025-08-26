using System.Threading.Tasks;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface ICajaService
    {
        Task<int> AbrirCajaAsync(int idUsuario, CajaAperturaCreate req);
        Task<int> RegistrarMovimientoAsync(int idUsuario, CajaMovimientoCreate req);
        Task<int> RealizarCorteAsync(int idUsuario, CajaCorteCreate req);
    }
}
