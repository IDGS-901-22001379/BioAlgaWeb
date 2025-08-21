using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _db;

        public ClienteRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // ============ BÚSQUEDA + FILTROS + Paginación ============
        public async Task<(IReadOnlyList<Cliente> Items, int Total)> BuscarAsync(ClienteQueryParams query)
        {
            // base query
            IQueryable<Cliente> q = _db.Set<Cliente>().AsNoTracking();

            // ---- Texto libre: nombre/apellidos/correo/telefono
            if (!string.IsNullOrWhiteSpace(query.Q))
            {
                var term = query.Q.Trim();
                var like = $"%{EscapeLike(term)}%";

                // Collation utf8mb4_unicode_ci ya es case-insensitive en MySQL,
                // pero usamos EF.Functions.Like para patrones.
                q = q.Where(c =>
                    EF.Functions.Like(c.Nombre, like) ||
                    EF.Functions.Like(c.ApellidoPaterno!, like) ||
                    EF.Functions.Like(c.ApellidoMaterno!, like) ||
                    EF.Functions.Like(c.Correo!, like) ||
                    EF.Functions.Like(c.Telefono!, like));
            }

            // ---- Filtros
            if (!string.IsNullOrWhiteSpace(query.Tipo_Cliente))
                q = q.Where(c => c.TipoCliente == query.Tipo_Cliente);

            if (!string.IsNullOrWhiteSpace(query.Estado))
                q = q.Where(c => c.Estado == query.Estado);

            // ---- Ordenamiento
            var sortBy  = (query.SortBy  ?? "nombre").ToLowerInvariant();
            var sortDir = (query.SortDir ?? "asc").ToLowerInvariant();

            q = sortBy switch
            {
                "fecha" => (sortDir == "desc")
                    ? q.OrderByDescending(c => c.FechaRegistro).ThenBy(c => c.IdCliente)
                    : q.OrderBy(c => c.FechaRegistro).ThenBy(c => c.IdCliente),

                _ => (sortDir == "desc")
                    ? q.OrderByDescending(c => c.Nombre)
                         .ThenByDescending(c => c.ApellidoPaterno)
                         .ThenByDescending(c => c.ApellidoMaterno)
                    : q.OrderBy(c => c.Nombre)
                         .ThenBy(c => c.ApellidoPaterno)
                         .ThenBy(c => c.ApellidoMaterno),
            };

            // ---- Total antes de paginar
            var total = await q.CountAsync();

            // ---- Paginación
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            var items = await q.Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync();

            return (items, total);
        }

        // ============ CRUD ============
        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _db.Set<Cliente>().AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdCliente == id);
        }

        public async Task<Cliente> AddAsync(Cliente cliente)
        {
            _db.Set<Cliente>().Add(cliente);
            await _db.SaveChangesAsync();
            return cliente;
        }

        public async Task<bool> UpdateAsync(Cliente cliente)
        {
            // Asegurar estado modificado
            _db.Set<Cliente>().Update(cliente);
            var changed = await _db.SaveChangesAsync();
            return changed > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _db.Set<Cliente>().FirstOrDefaultAsync(c => c.IdCliente == id);
            if (entity == null) return false;

            _db.Set<Cliente>().Remove(entity);
            var changed = await _db.SaveChangesAsync();
            return changed > 0;
        }

        // ============ Auxiliares ============
        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            var q = _db.Set<Cliente>().AsQueryable()
                       .Where(c => c.Correo == email);

            if (excludeId.HasValue)
                q = q.Where(c => c.IdCliente != excludeId.Value);

            return await q.AnyAsync();
        }

        // Escapa % y _ en patrones LIKE
        private static string EscapeLike(string input)
            => input.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_");
    }
}
