using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IProveedorRepository
    {
        Task<Proveedor?> GetByIdAsync(int id);
        Task<bool> ExistsByNombreAsync(string nombreEmpresa, int? exceptId = null);
        Task<Proveedor> AddAsync(Proveedor entity);
        Task UpdateAsync(Proveedor entity);
        Task DeleteAsync(Proveedor entity); // opcional (duro); usamos soft delete en servicio
        IQueryable<Proveedor> Query();      // para búsquedas/paginación
    }
}
