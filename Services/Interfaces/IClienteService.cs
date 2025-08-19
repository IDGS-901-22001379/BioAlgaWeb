// Services/Interfaces/IClienteService.cs
using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IClienteService
    {
        Task<(IEnumerable<ClienteDto> items, int total, int page, int pageSize)> 
            BuscarAsync(ClienteQueryParams q);

        Task<ClienteDto?> ObtenerPorIdAsync(int id);

        Task<ClienteDto> CrearAsync(ClienteCreateRequest req);

        Task<ClienteDto?> ActualizarAsync(int id, ClienteUpdateRequest req);

        Task<bool> EliminarAsync(int id);
    }
}
