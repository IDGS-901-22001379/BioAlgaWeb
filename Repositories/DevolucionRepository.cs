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

            // ⚠️ Corregido: filtro por producto
            if (qp.IdProducto.HasValue)
            {
                // Ejecutamos la query base primero
                var lista = await qry.ToListAsync(ct);

                // Luego filtramos en memoria (para evitar el error de primitive collection)
                lista = lista
                    .Where(d => d.Detalles.Any(x => x.IdProducto == qp.IdProducto.Value))
                    .ToList();

                // Reaplicamos orden y paginación en memoria
                var sort = (qp.SortBy ?? "fecha_desc").Trim().ToLowerInvariant();

                lista = sort switch
                {
                    "fecha_asc"  => lista.OrderBy(d => d.FechaDevolucion).ThenBy(d => d.IdDevolucion).ToList(),
                    "fecha_desc" => lista.OrderByDescending(d => d.FechaDevolucion).ThenByDescending(d => d.IdDevolucion).ToList(),
                    "total_asc"  => lista.OrderBy(d => d.TotalDevuelto).ThenBy(d => d.IdDevolucion).ToList(),
                    "total_desc" => lista.OrderByDescending(d => d.TotalDevuelto).ThenByDescending(d => d.IdDevolucion).ToList(),
                    _            => lista.OrderByDescending(d => d.FechaDevolucion).ThenByDescending(d => d.IdDevolucion).ToList()
                };

                var page = qp.Page <= 0 ? 1 : qp.Page;
                var size = qp.PageSize <= 0 ? 10 : qp.PageSize;

                var total = lista.Count;
                var items = lista.Skip((page - 1) * size).Take(size).ToList();

                return (items, total);
            }

            // ---------- Orden ----------
            var sortDefault = (qp.SortBy ?? "fecha_desc").Trim().ToLowerInvariant();

            qry = sortDefault switch
            {
                "fecha_asc"  => qry.OrderBy(d => d.FechaDevolucion).ThenBy(d => d.IdDevolucion),
                "fecha_desc" => qry.OrderByDescending(d => d.FechaDevolucion).ThenByDescending(d => d.IdDevolucion),

                "total_asc"  => qry.OrderBy(d => d.TotalDevuelto).ThenBy(d => d.IdDevolucion),
                "total_desc" => qry.OrderByDescending(d => d.TotalDevuelto).ThenByDescending(d => d.IdDevolucion),

                _            => qry.OrderByDescending(d => d.FechaDevolucion).ThenByDescending(d => d.IdDevolucion)
            };

            // ---------- Paginación ----------
            var pageDb = qp.Page <= 0 ? 1 : qp.Page;
            var sizeDb = qp.PageSize <= 0 ? 10 : qp.PageSize;

            var totalDb = await qry.CountAsync(ct);
            var itemsDb = await qry
                .Skip((pageDb - 1) * sizeDb)
                .Take(sizeDb)
                .ToListAsync(ct);

            return (itemsDb, totalDb);
        }

        public Task GuardarCambiosAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}
