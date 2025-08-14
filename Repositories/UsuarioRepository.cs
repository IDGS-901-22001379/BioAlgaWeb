using BioAlga.Backend.Data;
using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _ctx;
        public UsuarioRepository(ApplicationDbContext ctx) => _ctx = ctx;

        public async Task<Usuario?> GetByIdAsync(int id) =>
            await _ctx.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empleado)
                .FirstOrDefaultAsync(u => u.Id_Usuario == id);

        public async Task<bool> ExistsUserNameAsync(string userName) =>
            await _ctx.Usuarios.AnyAsync(u => u.Nombre_Usuario == userName);

        public async Task AddAsync(Usuario usuario) =>
            await _ctx.Usuarios.AddAsync(usuario);

        public void Update(Usuario usuario) =>
            _ctx.Usuarios.Update(usuario);

        public void Remove(Usuario usuario) =>
            _ctx.Usuarios.Remove(usuario);

        // Búsqueda por nombre de usuario y por nombre/apellidos del empleado; paginación y filtro por activo
        public async Task<(IReadOnlyList<Usuario> Items, int Total)> SearchAsync(UsuarioQueryParams q)
        {
            var query = _ctx.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Empleado)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Nombre))
            {
                var n = q.Nombre.Trim();
                query = query.Where(u =>
                    u.Nombre_Usuario.Contains(n) ||
                    (u.Empleado != null && (
                        (u.Empleado.Nombre ?? "").Contains(n) ||
                        (u.Empleado.Apellido_Paterno ?? "").Contains(n) ||
                        (u.Empleado.Apellido_Materno ?? "").Contains(n) ||
                        (
                            (u.Empleado.Nombre ?? "") + " " +
                            (u.Empleado.Apellido_Paterno ?? "") + " " +
                            (u.Empleado.Apellido_Materno ?? "")
                        ).Contains(n)
                    )));
            }

            if (q.Activo.HasValue)
                query = query.Where(u => u.Activo == q.Activo.Value);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.Nombre_Usuario)
                .ThenBy(u => u.Empleado!.Apellido_Paterno)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToListAsync();

            return (items, total);
        }

        public Task<int> SaveChangesAsync() => _ctx.SaveChangesAsync();
    }
}
