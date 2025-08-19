// Repositories/ClienteRepository.cs
using BioAlga.Backend.Data;
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

        // ---------------------------------------
        // Obtener todos con filtros + paginación
        // ---------------------------------------
        public async Task<IEnumerable<Cliente>> GetAllAsync(
            string? q = null,
            string? estado = null,
            string? tipoCliente = null,
            DateTime? desde = null,
            DateTime? hasta = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _db.Clientes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(c =>
                    (c.Nombre   ?? "").Contains(q) ||
                    (c.Apellido ?? "").Contains(q) ||
                    (c.Correo   ?? "").Contains(q) ||
                    (c.Telefono ?? "").Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(c => c.Estado == estado);

            if (!string.IsNullOrWhiteSpace(tipoCliente))
                query = query.Where(c => c.Tipo_Cliente == tipoCliente);

            if (desde.HasValue)
                query = query.Where(c => c.Fecha_Registro >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(c => c.Fecha_Registro <= hasta.Value);

            return await query
                .OrderByDescending(c => c.Fecha_Registro)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(
            string? q = null,
            string? estado = null,
            string? tipoCliente = null,
            DateTime? desde = null,
            DateTime? hasta = null)
        {
            var query = _db.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(c =>
                    (c.Nombre   ?? "").Contains(q) ||
                    (c.Apellido ?? "").Contains(q) ||
                    (c.Correo   ?? "").Contains(q) ||
                    (c.Telefono ?? "").Contains(q));
            }

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(c => c.Estado == estado);

            if (!string.IsNullOrWhiteSpace(tipoCliente))
                query = query.Where(c => c.Tipo_Cliente == tipoCliente);

            if (desde.HasValue)
                query = query.Where(c => c.Fecha_Registro >= desde.Value);

            if (hasta.HasValue)
                query = query.Where(c => c.Fecha_Registro <= hasta.Value);

            return await query.CountAsync();
        }

        // ---------------------------------------
        // Obtener por Id
        // ---------------------------------------
        public async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _db.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id_Cliente == id);
        }

        // ---------------------------------------
        // Crear
        // ---------------------------------------
        public async Task AddAsync(Cliente cliente)
        {
            await _db.Clientes.AddAsync(cliente);
        }

        // ---------------------------------------
        // Actualizar
        // ---------------------------------------
        public async Task UpdateAsync(Cliente cliente)
        {
            _db.Clientes.Update(cliente);
            await Task.CompletedTask;
        }

        // ---------------------------------------
        // Eliminar
        // ---------------------------------------
        public async Task DeleteAsync(Cliente cliente)
        {
            _db.Clientes.Remove(cliente);
            await Task.CompletedTask;
        }

        // ---------------------------------------
        // Guardar cambios
        // ---------------------------------------
        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
