using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IProveedorService
    {
        Task<ProveedorDto?> ObtenerPorIdAsync(int id);
        Task<PagedResponse<ProveedorDto>> BuscarAsync(ProveedorQueryParams p);
        Task<ProveedorDto> CrearAsync(CrearProveedorDto dto);
        Task<ProveedorDto?> ActualizarAsync(int id, ActualizarProveedorDto dto);
        Task<bool> CambiarEstatusAsync(int id, string nuevoEstatus); // Activo/Inactivo
        Task<bool> EliminarDuroAsync(int id); // si lo necesitas realmente
    }
}
