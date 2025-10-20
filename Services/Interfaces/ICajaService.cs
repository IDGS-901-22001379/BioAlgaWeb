using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface ICajaService
    {
        Task<IReadOnlyList<CajaDto>> ListarAsync();
        Task<CajaDto?> ObtenerPorIdAsync(int id);
        Task<CajaDto> CrearAsync(CrearCajaDto dto);
        Task<CajaDto?> ActualizarAsync(int id, ActualizarCajaDto dto);
        Task<bool> EliminarAsync(int id);
    }
}
