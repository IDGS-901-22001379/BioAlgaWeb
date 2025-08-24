using BioAlga.Backend.Data;
using BioAlga.Backend.Models;
using BioAlga.Backend.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BioAlga.Backend.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly ApplicationDbContext _db;
        public ProveedorRepository(ApplicationDbContext db) => _db = db;

        public IQueryable<Proveedor> Query() => _db.Proveedores.AsNoTracking();

        public async Task<Proveedor?> GetByIdAsync(int id) =>
            await _db.Proveedores.FindAsync(id);

        public async Task<bool> ExistsByNombreAsync(string nombreEmpresa, int? exceptId = null) =>
            await _db.Proveedores.AnyAsync(p =>
                p.NombreEmpresa == nombreEmpresa &&
                (!exceptId.HasValue || p.IdProveedor != exceptId.Value));

        public async Task<Proveedor> AddAsync(Proveedor entity)
        {
            _db.Proveedores.Add(entity);
            await _db.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Proveedor entity)
        {
            _db.Proveedores.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Proveedor entity)
        {
            _db.Proveedores.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
