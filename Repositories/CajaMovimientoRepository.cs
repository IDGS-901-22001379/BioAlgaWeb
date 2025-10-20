using BioAlga.Backend.Data;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class CajaMovimientoRepository : ICajaMovimientoRepository
    {
        private readonly ApplicationDbContext _db;
        public CajaMovimientoRepository(ApplicationDbContext db) => _db = db;

        public async Task<(IReadOnlyList<CajaMovimiento> Items, int Total)> BuscarPorTurnoAsync(
            int idTurno, string? tipo, string? qConcepto, int page = 1, int pageSize = 20)
        {
            IQueryable<CajaMovimiento> q = _db.Set<CajaMovimiento>()
                                              .AsNoTracking()
                                              .Where(m => m.IdTurno == idTurno);

            if (!string.IsNullOrWhiteSpace(tipo))
                q = q.Where(m => m.Tipo == tipo); // 'Ingreso' | 'Egreso'

            if (!string.IsNullOrWhiteSpace(qConcepto))
            {
                var term = qConcepto.Trim();
                var like = $"%{EscapeLike(term)}%";
                q = q.Where(m => EF.Functions.Like(m.Concepto, like));
            }

            q = q.OrderByDescending(m => m.Fecha).ThenByDescending(m => m.IdMov);

            var total = await q.CountAsync();

            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var items = await q.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            return (items, total);
        }

        public async Task<CajaMovimiento?> GetByIdAsync(int id)
            => await _db.Set<CajaMovimiento>().AsNoTracking().FirstOrDefaultAsync(x => x.IdMov == id);

        public async Task<CajaMovimiento> AddAsync(CajaMovimiento mov)
        {
            _db.Set<CajaMovimiento>().Add(mov);
            await _db.SaveChangesAsync();
            return mov;
        }

        public async Task<bool> UpdateAsync(CajaMovimiento mov)
        {
            _db.Set<CajaMovimiento>().Update(mov);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Set<CajaMovimiento>().FirstOrDefaultAsync(x => x.IdMov == id);
            if (entity is null) return false;
            _db.Set<CajaMovimiento>().Remove(entity);
            return await _db.SaveChangesAsync() > 0;
        }

        // Escapa % y _ en patrones LIKE
        private static string EscapeLike(string input)
            => input.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
    }
}
