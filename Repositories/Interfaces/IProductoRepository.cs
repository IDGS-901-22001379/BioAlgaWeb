using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IProductoRepository
    {
        Task<(IReadOnlyList<Producto> items, int total)> SearchAsync(ProductoQueryParams qp, CancellationToken ct = default);
        Task<Producto?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsSkuAsync(string sku, int? excludeId = null, CancellationToken ct = default);
        Task<Producto> AddAsync(Producto entity, CancellationToken ct = default);
        Task UpdateAsync(Producto entity, CancellationToken ct = default);
        Task DeleteAsync(Producto entity, CancellationToken ct = default); // puedes inactivar en Service
    }
}
