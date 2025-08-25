// Repositories/PrecioRepository.cs
using System.Linq;                          // LINQ
using System.Threading;                     // CancellationToken
using System.Threading.Tasks;               // Task<>
using BioAlga.Backend.Data;                 // ApplicationDbContext
using BioAlga.Backend.Models;               // ProductoPrecio
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class PrecioRepository : IPrecioRepository
    {
        private readonly ApplicationDbContext _db;
        public PrecioRepository(ApplicationDbContext db) => _db = db;

        public async Task<IReadOnlyList<ProductoPrecio>> GetHistorialAsync(
            int idProducto,
            CancellationToken ct = default)
        {
            var list = await _db.ProductoPrecios.AsNoTracking()
                .Where(x => x.IdProducto == idProducto)
                .OrderByDescending(x => x.VigenteDesde)
                .ToListAsync(ct);

            return list;
        }

        public async Task<IReadOnlyList<ProductoPrecio>> GetVigentesAsync(
            int idProducto,
            CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            // Toma 0..1 vigente por tipo (Normal, Mayoreo, Descuento, Especial)
            var vigentes = await _db.ProductoPrecios.AsNoTracking()
                .Where(x => x.IdProducto == idProducto
                         && x.Activo
                         && (x.VigenteHasta == null || x.VigenteHasta >= now))
                .GroupBy(x => new { x.IdProducto, x.TipoPrecio })
                .Select(g => g.OrderByDescending(x => x.VigenteDesde).First())
                .ToListAsync(ct);

            return vigentes;
        }

        public Task<ProductoPrecio?> GetByIdAsync(
            int idPrecio,
            CancellationToken ct = default)
        {
            return _db.ProductoPrecios
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdPrecio == idPrecio, ct);
        }

        public async Task<ProductoPrecio> AddAsync(
            ProductoPrecio entity,
            CancellationToken ct = default)
        {
            _db.ProductoPrecios.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(
            ProductoPrecio entity,
            CancellationToken ct = default)
        {
            _db.ProductoPrecios.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DesactivarVigenteDelMismoTipoAsync(
            int idProducto,
            string tipoPrecio,
            DateTime? cerrarHasta = null,
            CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            var vigentesMismoTipo = await _db.ProductoPrecios
                .Where(x => x.IdProducto == idProducto
                         && x.TipoPrecio == tipoPrecio
                         && x.Activo
                         && (x.VigenteHasta == null || x.VigenteHasta >= now))
                .ToListAsync(ct);

            foreach (var v in vigentesMismoTipo)
            {
                v.Activo = false;
                v.VigenteHasta = cerrarHasta ?? now;
                _db.ProductoPrecios.Update(v);
            }

            await _db.SaveChangesAsync(ct);
        }

        public Task<bool> HayVigenteDelMismoTipoAsync(
            int idProducto,
            string tipoPrecio,
            int? excludeId = null,
            CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            return _db.ProductoPrecios.AnyAsync(x =>
                x.IdProducto == idProducto
                && x.TipoPrecio == tipoPrecio
                && x.Activo
                && (x.VigenteHasta == null || x.VigenteHasta >= now)
                && (excludeId == null || x.IdPrecio != excludeId.Value),
                ct);
        }
    }
}
