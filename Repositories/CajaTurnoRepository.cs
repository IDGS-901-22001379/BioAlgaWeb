using BioAlga.Backend.Data;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class CajaTurnoRepository : ICajaTurnoRepository
    {
        private readonly ApplicationDbContext _db;
        public CajaTurnoRepository(ApplicationDbContext db) => _db = db;

        public async Task<CajaTurno?> GetByIdAsync(int id)
            => await _db.Set<CajaTurno>()
                        .AsNoTracking()
                        .Include(x => x.Caja)
                        .Include(x => x.Usuario)
                        .FirstOrDefaultAsync(x => x.IdTurno == id);

        public async Task<CajaTurno?> GetTurnoAbiertoPorCajaAsync(int idCaja)
            => await _db.Set<CajaTurno>()
                        .AsNoTracking()
                        .Include(x => x.Caja)
                        .Include(x => x.Usuario)
                        .Where(x => x.IdCaja == idCaja && x.Cierre == null)
                        .OrderByDescending(x => x.Apertura)
                        .FirstOrDefaultAsync();

        public async Task<CajaTurno?> GetTurnoAbiertoPorUsuarioAsync(int idUsuario)
            => await _db.Set<CajaTurno>()
                        .AsNoTracking()
                        .Include(x => x.Caja)
                        .Include(x => x.Usuario)
                        .Where(x => x.IdUsuario == idUsuario && x.Cierre == null)
                        .OrderByDescending(x => x.Apertura)
                        .FirstOrDefaultAsync();

        public async Task<CajaTurno> AbrirTurnoAsync(CajaTurno turno)
        {
            _db.Set<CajaTurno>().Add(turno);
            await _db.SaveChangesAsync();
            return turno;
        }

        public async Task<bool> CerrarTurnoAsync(CajaTurno turno)
        {
            _db.Set<CajaTurno>().Update(turno);
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<(IReadOnlyList<CajaTurno> Items, int Total)> BuscarAsync(
            int? idCaja, int? idUsuario, DateTime? desde, DateTime? hasta,
            int page = 1, int pageSize = 10)
        {
            IQueryable<CajaTurno> q = _db.Set<CajaTurno>()
                                         .AsNoTracking()
                                         .Include(x => x.Caja)
                                         .Include(x => x.Usuario);

            if (idCaja.HasValue) q = q.Where(x => x.IdCaja == idCaja.Value);
            if (idUsuario.HasValue) q = q.Where(x => x.IdUsuario == idUsuario.Value);
            if (desde.HasValue) q = q.Where(x => x.Apertura >= desde.Value);
            if (hasta.HasValue) q = q.Where(x => (x.Cierre ?? DateTime.MaxValue) <= hasta.Value);

            q = q.OrderByDescending(x => x.Apertura)
                 .ThenByDescending(x => x.IdTurno);

            var total = await q.CountAsync();

            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 10 : pageSize;

            var items = await q.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            return (items, total);
        }
    }
}
