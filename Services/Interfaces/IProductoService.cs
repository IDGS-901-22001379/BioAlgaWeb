using BioAlga.Backend.Dtos;

namespace BioAlga.Backend.Services.Interfaces
{
    public interface IProductoService
    {
        /// <summary>
        /// Busca productos con filtros avanzados (paginación, categoría, etc.).
        /// </summary>
        Task<PagedResponse<ProductoDto>> BuscarAsync(ProductoQueryParams qp, CancellationToken ct = default);

        /// <summary>
        /// Obtiene un producto por su Id.
        /// </summary>
        Task<ProductoDto?> ObtenerPorIdAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Crea un nuevo producto en el catálogo.
        /// </summary>
        Task<ProductoDto> CrearAsync(CrearProductoDto dto, CancellationToken ct = default);

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        Task<bool> ActualizarAsync(int id, ActualizarProductoDto dto, CancellationToken ct = default);

        /// <summary>
        /// Elimina (o inactiva) un producto del catálogo.
        /// </summary>
        Task<bool> EliminarAsync(int id, CancellationToken ct = default);

        /// <summary>
        /// Búsqueda rápida/autocomplete por nombre, SKU o código de barras.
        /// Devuelve una lista corta de coincidencias activas.
        /// </summary>
        Task<List<ProductoLookupDto>> BuscarBasicoAsync(string q, int limit = 10);
    }
}
