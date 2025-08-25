using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces;

public interface ICompraRepository
{
    Task<Compra> CreateAsync(Compra compra);
    Task<Compra?> GetAsync(int id);
    Task<(IEnumerable<Compra> items, int total)> SearchAsync(string? q, DateTime? desde, DateTime? hasta, int page, int pageSize);
    Task AddDetalleAsync(DetalleCompra detalle);
    Task RemoveDetalleAsync(int idDetalle);
    Task SaveAsync();
}
