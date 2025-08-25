// Repositories/ProductoRepository.cs
using System.Linq;                          // LINQ (Where, OrderBy, etc.)
using System.Threading;                     // CancellationToken
using System.Threading.Tasks;               // Task<>
using BioAlga.Backend.Data;                 // ApplicationDbContext
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductoRepository(ApplicationDbContext db) => _db = db;

        public async Task<(IReadOnlyList<Producto> items, int total)> SearchAsync(
            ProductoQueryParams qp,
            CancellationToken ct = default)
        {
            // base
            IQueryable<Producto> q = _db.Productos.AsNoTracking();

            // q: nombre / sku / código de barras (case-insensitive; trim & spaces tolerant)
            if (!string.IsNullOrWhiteSpace(qp.Q))
            {
                var term = qp.Q.Trim().ToLower();
                q = q.Where(p =>
                    EF.Functions.Like(p.Nombre.ToLower(), $"%{term}%") ||
                    p.CodigoSku.ToLower().Contains(term) ||
                    (p.CodigoBarras != null && p.CodigoBarras.ToLower().Contains(term))
                );
            }

            // filtros
            if (!string.IsNullOrWhiteSpace(qp.Tipo))
                q = q.Where(p => p.Tipo.ToString() == qp.Tipo);

            if (qp.IdCategoria.HasValue) q = q.Where(p => p.IdCategoria == qp.IdCategoria);
            if (qp.IdMarca.HasValue)     q = q.Where(p => p.IdMarca == qp.IdMarca);
            if (qp.IdUnidad.HasValue)    q = q.Where(p => p.IdUnidad == qp.IdUnidad);
            if (!string.IsNullOrWhiteSpace(qp.Estatus))
                q = q.Where(p => p.Estatus == qp.Estatus);

            // sorting seguro
            q = ApplySort(q, qp.SortBy, qp.SortDir);

            // total
            var total = await q.CountAsync(ct);

            // paginado
            int page = Math.Max(qp.Page, 1);
            int size = Math.Clamp(qp.PageSize, 1, 100);
            var items = await q.Skip((page - 1) * size).Take(size).ToListAsync(ct);

            return (items, total);
        }

        private static IQueryable<Producto> ApplySort(IQueryable<Producto> q, string? sortBy, string? sortDir)
        {
            bool desc = (sortDir?.ToLower() == "desc");
            return (sortBy?.ToLower()) switch
            {
                "sku" or "codigosku" => desc ? q.OrderByDescending(p => p.CodigoSku) : q.OrderBy(p => p.CodigoSku),
                "created_at"         => desc ? q.OrderByDescending(p => p.Created_At) : q.OrderBy(p => p.Created_At),
                "updated_at"         => desc ? q.OrderByDescending(p => p.Updated_At) : q.OrderBy(p => p.Updated_At),
                _                    => desc ? q.OrderByDescending(p => p.Nombre)     : q.OrderBy(p => p.Nombre),
            };
        }

        public async Task<Producto?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Productos
                .Include(p => p.Especificaciones)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == id, ct);
        }

        public Task<bool> ExistsSkuAsync(string sku, int? excludeId = null, CancellationToken ct = default)
        {
            return _db.Productos.AnyAsync(
                p => p.CodigoSku == sku && (excludeId == null || p.IdProducto != excludeId.Value),
                ct
            );
        }

        public async Task<Producto> AddAsync(Producto entity, CancellationToken ct = default)
        {
            _db.Productos.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(Producto entity, CancellationToken ct = default)
        {
            _db.Productos.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Producto entity, CancellationToken ct = default)
        {
            _db.Productos.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
