using BioAlga.Backend.Data;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class CajaRepository : ICajaRepository
    {
        private readonly ApplicationDbContext _db;
        public CajaRepository(ApplicationDbContext db) => _db = db;

        public async Task<IReadOnlyList<Caja>> GetAllAsync()
            => await _db.Set<Caja>().AsNoTracking().OrderBy(x => x.Nombre).ToListAsync();

        public async Task<Caja?> GetByIdAsync(int id)
            => await _db.Set<Caja>().AsNoTracking().FirstOrDefaultAsync(x => x.IdCaja == id);

        public async Task<Caja> AddAsync(Caja caja)
        {
            _db.Set<Caja>().Add(caja);
            await _db.SaveChangesAsync();
            return caja;
        }

        public async Task<bool> UpdateAsync(Caja caja)
        {
            _db.Set<Caja>().Update(caja);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Set<Caja>().FirstOrDefaultAsync(x => x.IdCaja == id);
            if (entity is null) return false;
            _db.Set<Caja>().Remove(entity);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<bool> NombreExistsAsync(string nombre, int? excludeId = null)
        {
            var q = _db.Set<Caja>().AsQueryable().Where(x => x.Nombre == nombre);
            if (excludeId.HasValue) q = q.Where(x => x.IdCaja != excludeId.Value);
            return await q.AnyAsync();
        }
    }
}
