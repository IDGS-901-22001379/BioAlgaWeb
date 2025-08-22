using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using Microsoft.EntityFrameworkCore;
using BioAlga.Backend.Repositories.Interfaces;

namespace BioAlga.Backend.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly ApplicationDbContext _db;

        public EmpleadoRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        // =========================
        // CRUD
        // =========================
        public async Task<Empleado?> GetByIdAsync(int id)
        {
            // Incluye Usuario si te sirve (puedes quitar Include si no lo necesitas)
            return await _db.Empleados
                .Include(e => e.Usuario)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id_Empleado == id);
        }

        public async Task AddAsync(Empleado empleado)
        {
            await _db.Empleados.AddAsync(empleado);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Empleado empleado)
        {
            _db.Empleados.Update(empleado);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Empleado empleado)
        {
            _db.Empleados.Remove(empleado);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Empleados.AnyAsync(e => e.Id_Empleado == id);
        }

        // =========================
        // SEARCH + PAGINACIÓN
        // =========================
        public async Task<(IReadOnlyList<Empleado> items, int total)> SearchAsync(EmpleadoQueryParams query)
        {
            // Base query
            var q = _db.Empleados.AsNoTracking().AsQueryable();

            // --- Filtro por texto libre (nombre/apellidos)
            if (!string.IsNullOrWhiteSpace(query.q))
            {
                var term = $"%{query.q.Trim()}%";
                q = q.Where(e =>
                    EF.Functions.Like(e.Nombre, term) ||
                    EF.Functions.Like(e.Apellido_Paterno!, term) ||
                    EF.Functions.Like(e.Apellido_Materno!, term));
            }

            // --- Filtro por puesto (exacto)
            if (!string.IsNullOrWhiteSpace(query.puesto))
            {
                q = q.Where(e => e.Puesto == query.puesto);
            }

            // --- Filtro por estatus
            if (!string.IsNullOrWhiteSpace(query.estatus))
            {
                q = q.Where(e => e.Estatus == query.estatus);
            }

            // --- Total antes de paginar
            var total = await q.CountAsync();

            // --- Ordenamiento
            var sortBy = (query.sortBy ?? "nombre").ToLower();
            var sortDir = (query.sortDir ?? "ASC").ToUpper();

            q = (sortBy, sortDir) switch
            {
                ("fecha_ingreso", "DESC") => q.OrderByDescending(e => e.Fecha_Ingreso).ThenBy(e => e.Id_Empleado),
                ("fecha_ingreso", _)      => q.OrderBy(e => e.Fecha_Ingreso).ThenBy(e => e.Id_Empleado),

                ("salario", "DESC")       => q.OrderByDescending(e => e.Salario).ThenBy(e => e.Id_Empleado),
                ("salario", _)            => q.OrderBy(e => e.Salario).ThenBy(e => e.Id_Empleado),

                ("puesto", "DESC")        => q.OrderByDescending(e => e.Puesto).ThenBy(e => e.Id_Empleado),
                ("puesto", _)             => q.OrderBy(e => e.Puesto).ThenBy(e => e.Id_Empleado),

                ("estatus", "DESC")       => q.OrderByDescending(e => e.Estatus).ThenBy(e => e.Id_Empleado),
                ("estatus", _)            => q.OrderBy(e => e.Estatus).ThenBy(e => e.Id_Empleado),

                // default: nombre + apellidos
                ("nombre", "DESC")        => q.OrderByDescending(e => e.Nombre)
                                              .ThenByDescending(e => e.Apellido_Paterno)
                                              .ThenByDescending(e => e.Apellido_Materno)
                                              .ThenBy(e => e.Id_Empleado),
                _                         => q.OrderBy(e => e.Nombre)
                                              .ThenBy(e => e.Apellido_Paterno)
                                              .ThenBy(e => e.Apellido_Materno)
                                              .ThenBy(e => e.Id_Empleado),
            };

            // --- Paginación
            var page = query.page <= 0 ? 1 : query.page;
            var size = query.pageSize <= 0 ? 10 : query.pageSize;

            var items = await q
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return (items, total);
        }
    }
}
