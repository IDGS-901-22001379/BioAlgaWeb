using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IDevolucionRepository
    {
        // Crear (cabecera + detalles vía navegación)
        Task CrearAsync(Devolucion entity, CancellationToken ct = default);

        // Traer por id con detalles
        Task<Devolucion?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

        // Búsqueda paginada con filtros (coincide con DevolucionQueryParams)
        Task<(IReadOnlyList<Devolucion> items, int total)>
            BuscarAsync(DevolucionQueryParams qp, CancellationToken ct = default);

        // Confirmar cambios
        Task GuardarCambiosAsync(CancellationToken ct = default);
    }
}
