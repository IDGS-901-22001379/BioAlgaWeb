using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IDevolucionService
    {
        /// <summary>
        /// Crea una devolución (sin obligar número de venta).
        /// - Calcula TotalDevuelto a partir de las líneas.
        /// - Congela UsuarioNombre y ProductoNombre.
        /// - Si RegresaInventario = true, genera movimiento de inventario "Entrada" (origen: Devolucion).
        /// </summary>
        Task<DevolucionDto> CrearAsync(int idUsuario, DevolucionCreateRequest req, CancellationToken ct = default);

        /// <summary>
        /// Obtiene una devolución por id (incluye detalles).
        /// </summary>
        Task<DevolucionDto?> ObtenerAsync(int idDevolucion, CancellationToken ct = default);

        /// <summary>
        /// Búsqueda paginada con filtros/orden.
        /// </summary>
        Task<PagedResponse<DevolucionDto>> BuscarAsync(DevolucionQueryParams qp, CancellationToken ct = default);
    }
}
