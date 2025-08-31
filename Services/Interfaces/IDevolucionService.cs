using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IDevolucionService
    {
        Task<DevolucionDto> RegistrarDevolucionAsync(int idUsuario, DevolucionCreateRequest req);
        Task<DevolucionDto?> ObtenerPorIdAsync(int idDevolucion);
        Task<List<DevolucionDto>> ListarAsync(DevolucionQueryParams filtro);
    }
}
