using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IEmpleadoService
    {
        Task<EmpleadoDto?> ObtenerPorIdAsync(int id);

        /// <summary>
        /// Búsqueda con filtros, orden y paginación.
        /// </summary>
        Task<PagedResponse<EmpleadoDto>> BuscarAsync(EmpleadoQueryParams query);

        Task<EmpleadoDto> CrearAsync(CrearEmpleadoDto dto);

        /// <returns>true si actualizó; false si no existe</returns>
        Task<bool> ActualizarAsync(ActualizarEmpleadoDto dto);

        /// <returns>true si eliminó; false si no existe</returns>
        Task<bool> EliminarAsync(int id);
    }
}
