using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class VentaRepository : IVentaRepository
    {
        private readonly ApplicationDbContext _db;

        public VentaRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // ===========================
        // BÚSQUEDA / HISTORIAL
        // ===========================
        public async Task<(IReadOnlyList<Venta> Items, int Total)> SearchAsync(
            VentaQueryParams qp, CancellationToken ct = default)
        {
            // Normaliza strings de fecha si llegaron así
            qp.NormalizeDates();

            // Parse de enums que vienen como string
            BioAlga.Backend.Models.Enums.EstatusVenta? estatusParsed = null;
            if (!string.IsNullOrWhiteSpace(qp.Estatus) &&
                Enum.TryParse<BioAlga.Backend.Models.Enums.EstatusVenta>(qp.Estatus, true, out var e))
                estatusParsed = e;

            BioAlga.Backend.Models.Enums.MetodoPago? metodoParsed = null;
            if (!string.IsNullOrWhiteSpace(qp.MetodoPago) &&
                Enum.TryParse<BioAlga.Backend.Models.Enums.MetodoPago>(qp.MetodoPago, true, out var m))
                metodoParsed = m;

            // Paginación por defecto
            var page     = qp.Page     <= 0 ? 1  : qp.Page;
            var pageSize = qp.PageSize <= 0 ? 10 : qp.PageSize;

            IQueryable<Venta> q = _db.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.Detalles); // <- si tu propiedad es Detalles, cámbiala aquí

            // ---------- Filtros ----------
            if (qp.ClienteId.HasValue)
                q = q.Where(v => v.ClienteId == qp.ClienteId.Value);

            if (estatusParsed.HasValue)
                q = q.Where(v => v.Estatus == estatusParsed.Value);

            if (metodoParsed.HasValue)
                q = q.Where(v => v.MetodoPago == metodoParsed.Value);

            if (qp.UsuarioId.HasValue)
                q = q.Where(v => v.IdUsuario == qp.UsuarioId.Value);

            if (qp.Desde.HasValue)
                q = q.Where(v => v.FechaVenta >= qp.Desde.Value);

            if (qp.Hasta.HasValue)
                q = q.Where(v => v.FechaVenta <= qp.Hasta.Value);

            // Búsqueda libre por cliente o por Id de venta
            if (!string.IsNullOrWhiteSpace(qp.Q))
            {
                var txt = qp.Q.Trim();
                q = q.Where(v =>
                    v.IdVenta.ToString().Contains(txt) ||
                    (v.Cliente != null &&
                        ((v.Cliente.Nombre ?? "").Contains(txt) ||
                         (v.Cliente.ApellidoPaterno ?? "").Contains(txt) ||
                         (v.Cliente.ApellidoMaterno ?? "").Contains(txt)))
                );
            }

            // ---------- Orden ----------
            var sortBy  = qp.SortBy?.ToLowerInvariant() ?? "fecha_venta";
            var sortDir = qp.SortDir?.ToLowerInvariant() == "asc" ? "asc" : "desc";

            q = sortBy switch
            {
                "total"       => (sortDir == "asc" ? q.OrderBy(v => v.Total)       : q.OrderByDescending(v => v.Total)),
                "subtotal"    => (sortDir == "asc" ? q.OrderBy(v => v.Subtotal)    : q.OrderByDescending(v => v.Subtotal)),
                "impuestos"   => (sortDir == "asc" ? q.OrderBy(v => v.Impuestos)   : q.OrderByDescending(v => v.Impuestos)),
                "metodo_pago" => (sortDir == "asc" ? q.OrderBy(v => v.MetodoPago)  : q.OrderByDescending(v => v.MetodoPago)),
                "estatus"     => (sortDir == "asc" ? q.OrderBy(v => v.Estatus)     : q.OrderByDescending(v => v.Estatus)),
                "id_venta"    => (sortDir == "asc" ? q.OrderBy(v => v.IdVenta)     : q.OrderByDescending(v => v.IdVenta)),
                _             => (sortDir == "asc" ? q.OrderBy(v => v.FechaVenta)  : q.OrderByDescending(v => v.FechaVenta)),
            };

            // ---------- Total + Paginación ----------
            var total = await q.CountAsync(ct);

            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        // ===========================
        // DETALLE DE UNA VENTA
        // ===========================
        public async Task<Venta?> GetByIdWithDetailsAsync(int idVenta, CancellationToken ct = default)
        {
            return await _db.Ventas
                .AsNoTracking()
                .Include(v => v.Cliente)
                .Include(v => v.Usuario)
                .Include(v => v.Detalles)            // <- si tu propiedad es Detalles, cámbiala aquí
                    .ThenInclude(d => d.Producto)   // para nombre y SKU
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta, ct);
        }
    }
}
