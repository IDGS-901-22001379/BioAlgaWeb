// Repositories/Interfaces/IClienteRepository.cs
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        // Obtener todos (con filtros opcionales)
        Task<IEnumerable<Cliente>> GetAllAsync(
            string? q = null,
            string? estado = null,
            string? tipoCliente = null,
            DateTime? desde = null,
            DateTime? hasta = null,
            int page = 1,
            int pageSize = 10);

        Task<int> CountAsync(
            string? q = null,
            string? estado = null,
            string? tipoCliente = null,
            DateTime? desde = null,
            DateTime? hasta = null);

        // Obtener uno por Id
        Task<Cliente?> GetByIdAsync(int id);

        // Crear
        Task AddAsync(Cliente cliente);

        // Actualizar
        Task UpdateAsync(Cliente cliente);

        // Eliminar
        Task DeleteAsync(Cliente cliente);

        // Guardar cambios
        Task SaveChangesAsync();
    }
}
