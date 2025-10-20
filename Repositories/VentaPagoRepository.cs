using BioAlga.Backend.Data;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class VentaPagoRepository : IVentaPagoRepository
    {
        private readonly ApplicationDbContext _db;
        public VentaPagoRepository(ApplicationDbContext db) => _db = db;

        public async Task<IReadOnlyList<VentaPago>> GetByVentaAsync(int idVenta)
            => await _db.Set<VentaPago>()
                        .AsNoTracking()
                        .Where(p => p.IdVenta == idVenta)
                        .OrderBy(p => p.IdPago)
                        .ToListAsync();

        public async Task<VentaPago> AddAsync(VentaPago pago)
        {
            _db.Set<VentaPago>().Add(pago);
            await _db.SaveChangesAsync();
            return pago;
        }

        public async Task<bool> DeleteAsync(int idPago)
        {
            var entity = await _db.Set<VentaPago>().FirstOrDefaultAsync(p => p.IdPago == idPago);
            if (entity is null) return false;
            _db.Set<VentaPago>().Remove(entity);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<decimal> TotalPorMetodoAsync(int idVenta, string metodo)
            => await _db.Set<VentaPago>()
                        .Where(p => p.IdVenta == idVenta && p.Metodo == metodo)
                        .SumAsync(p => (decimal?)p.Monto) ?? 0m;
    }
}
