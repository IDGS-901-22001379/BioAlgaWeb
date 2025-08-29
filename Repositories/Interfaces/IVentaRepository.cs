using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IVentaRepository
    {
        /// <summary>
        /// Devuelve ventas filtradas + paginadas (sin incluir Producto en las líneas).
        /// Regresa también el total para construir el paginado.
        /// </summary>
        Task<(IReadOnlyList<Venta> Items, int Total)> SearchAsync(VentaQueryParams qp, CancellationToken ct = default);

        /// <summary>
        /// Trae una venta por Id con todos sus detalles y Producto (para “Ver detalles”).
        /// </summary>
        Task<Venta?> GetByIdWithDetailsAsync(int idVenta, CancellationToken ct = default);
    }
}
