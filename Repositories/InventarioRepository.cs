using BioAlga.Backend.Data;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class InventarioRepository : IInventarioRepository
    {
        private readonly ApplicationDbContext _db;
        public InventarioRepository(ApplicationDbContext db) => _db = db;

        // Implementa exactamente lo que pide la interfaz
        public async Task AddMovimientoAsync(InventarioMovimiento mov, CancellationToken ct = default)
        {
            if (mov.Fecha == default) mov.Fecha = DateTime.UtcNow;
            await _db.InventarioMovimientos.AddAsync(mov, ct);
        }

        public Task GuardarCambiosAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public async Task<int> StockActualAsync(int idProducto, CancellationToken ct = default)
        {
            var entradas = await _db.InventarioMovimientos
                .Where(m => m.IdProducto == idProducto && m.TipoMovimiento == "Entrada")
                .SumAsync(m => (int?)m.Cantidad, ct) ?? 0;

            var salidas = await _db.InventarioMovimientos
                .Where(m => m.IdProducto == idProducto && m.TipoMovimiento == "Salida")
                .SumAsync(m => (int?)m.Cantidad, ct) ?? 0;

            var ajustes = await _db.InventarioMovimientos
                .Where(m => m.IdProducto == idProducto && m.TipoMovimiento == "Ajuste")
                .SumAsync(m => (int?)m.Cantidad, ct) ?? 0;

            return entradas - salidas + ajustes;
        }
    }
}
