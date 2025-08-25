using BioAlga.Backend.Data;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories;

public class InventarioRepository : IInventarioRepository
{
    private readonly ApplicationDbContext _db;
    public InventarioRepository(ApplicationDbContext db) => _db = db;

    public async Task AddMovimientoAsync(InventarioMovimiento mov)
    {
        _db.InventarioMovimientos.Add(mov);
        await _db.SaveChangesAsync();
    }

    public async Task<int> StockActualAsync(int idProducto)
    {
        var entradas = await _db.InventarioMovimientos
            .Where(m => m.IdProducto == idProducto && m.TipoMovimiento == "Entrada")
            .SumAsync(m => (int?)m.Cantidad) ?? 0;

        var salidas = await _db.InventarioMovimientos
            .Where(m => m.IdProducto == idProducto && m.TipoMovimiento == "Salida")
            .SumAsync(m => (int?)m.Cantidad) ?? 0;

        var ajustes = await _db.InventarioMovimientos
            .Where(m => m.IdProducto == idProducto && m.TipoMovimiento == "Ajuste")
            .SumAsync(m => (int?)m.Cantidad) ?? 0;

        return entradas - salidas + ajustes;
    }
}
