using System.Threading.Tasks;
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IDevolucionService
    {
        Task<int> RegistrarDevolucionAsync(int idUsuario, DevolucionCreateRequest req);
    }
}
