using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces;

public interface IInventarioRepository
{
    Task AddMovimientoAsync(InventarioMovimiento mov);
    Task<int> StockActualAsync(int idProducto);
}
