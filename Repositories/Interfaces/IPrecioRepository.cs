using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IPrecioRepository
    {
        Task<IReadOnlyList<ProductoPrecio>> GetHistorialAsync(int idProducto, CancellationToken ct = default);
        Task<IReadOnlyList<ProductoPrecio>> GetVigentesAsync(int idProducto, CancellationToken ct = default);
        Task<ProductoPrecio?> GetByIdAsync(int idPrecio, CancellationToken ct = default);
        Task<ProductoPrecio> AddAsync(ProductoPrecio entity, CancellationToken ct = default);
        Task UpdateAsync(ProductoPrecio entity, CancellationToken ct = default);

        // Ayudantes para la regla “0..1 vigente por tipo/prod”
        Task DesactivarVigenteDelMismoTipoAsync(int idProducto, string tipoPrecio, DateTime? cerrarHasta = null, CancellationToken ct = default);
        Task<bool> HayVigenteDelMismoTipoAsync(int idProducto, string tipoPrecio, int? excludeId = null, CancellationToken ct = default);
    }
}
