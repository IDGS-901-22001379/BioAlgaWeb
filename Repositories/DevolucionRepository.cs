using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class DevolucionRepository : IDevolucionRepository
    {
        private readonly ApplicationDbContext _db;

        public DevolucionRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task CrearAsync(Devolucion entity, CancellationToken ct = default)
        {
            // EF Core inserta cabecera + detalles por navegación
            await _db.Devoluciones.AddAsync(entity, ct);
        }

        public async Task<Devolucion?> ObtenerPorIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Devoluciones
                .AsNoTracking()
                .Include(d => d.Detalles)
                .FirstOrDefaultAsync(d => d.IdDevolucion == id, ct);
        }

        public async Task<(IReadOnlyList<Devolucion> items, int total)>
            BuscarAsync(DevolucionQueryParams qp, CancellationToken ct = default)
        {
            var qry = _db.Devoluciones
                .AsNoTracking()
                .Include(d => d.Detalles)
                .AsQueryable();

            // ---------- Filtros ----------
            if (!string.IsNullOrWhiteSpace(qp.Q))
            {
                var q = qp.Q.Trim();
                qry = qry.Where(d =>
                    d.Motivo.Contains(q) ||
                    (d.ReferenciaVenta != null && d.ReferenciaVenta.Contains(q)) ||
                    (d.UsuarioNombre != null && d.UsuarioNombre.Contains(q)));
            }

            if (qp.IdUsuario.HasValue)
                qry = qry.Where(d => d.IdUsuario == qp.IdUsuario.Value);

            if (qp.RegresaInventario.HasValue)
                qry = qry.Where(d => d.RegresaInventario == qp.RegresaInventario.Value);

            if (qp.Desde.HasValue)
                qry = qry.Where(d => d.FechaDevolucion >= qp.Desde.Value);

            if (qp.Hasta.HasValue)
            {
                var hasta = qp.Hasta.Value;
                // Si viene solo fecha, incluir todo el día
                if (hasta.TimeOfDay == TimeSpan.Zero)
                    hasta = hasta.AddDays(1).AddTicks(-1);

                qry = qry.Where(d => d.FechaDevolucion <= hasta);
            }

            if (qp.IdProducto.HasValue)
                qry = qry.Where(d => d.Detalles.Any(x => x.IdProducto == qp.IdProducto.Value));

            // ---------- Orden ----------
            // qp.SortBy admite: "fecha_desc" (default), "fecha_asc", "total_desc", "total_asc"
            var sort = (qp.SortBy ?? "fecha_desc").Trim().ToLowerInvariant();

            qry = sort switch
            {
                "fecha_asc"  => qry.OrderBy(d => d.FechaDevolucion).ThenBy(d => d.IdDevolucion),
                "fecha_desc" => qry.OrderByDescending(d => d.FechaDevolucion).ThenByDescending(d => d.IdDevolucion),

                "total_asc"  => qry.OrderBy(d => d.TotalDevuelto).ThenBy(d => d.IdDevolucion),
                "total_desc" => qry.OrderByDescending(d => d.TotalDevuelto).ThenByDescending(d => d.IdDevolucion),

                _            => qry.OrderByDescending(d => d.FechaDevolucion).ThenByDescending(d => d.IdDevolucion)
            };

            // ---------- Paginación ----------
            var page = qp.Page <= 0 ? 1 : qp.Page;
            var size = qp.PageSize <= 0 ? 10 : qp.PageSize;

            var total = await qry.CountAsync(ct);
            var items = await qry
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct);

            return (items, total);
        }

        public Task GuardarCambiosAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}
