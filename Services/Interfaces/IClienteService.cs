using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IClienteService
    {
        Task<PagedResponse<ClienteDto>> BuscarAsync(ClienteQueryParams query);
        Task<ClienteDto?> ObtenerPorIdAsync(int id);
        Task<ClienteDto> CrearAsync(CrearClienteDto dto);
        Task<ClienteDto?> ActualizarAsync(int id, ActualizarClienteDto dto);
        Task<bool> EliminarAsync(int id);
    }
}
