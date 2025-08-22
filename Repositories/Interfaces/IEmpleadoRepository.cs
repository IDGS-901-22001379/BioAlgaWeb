using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IEmpleadoRepository
    {
        Task<Empleado?> GetByIdAsync(int id);
        Task AddAsync(Empleado empleado);
        Task UpdateAsync(Empleado empleado);
        Task DeleteAsync(Empleado empleado);
        Task<bool> ExistsAsync(int id);
        Task<(IReadOnlyList<Empleado> items, int total)> SearchAsync(EmpleadoQueryParams query);
    }
}

