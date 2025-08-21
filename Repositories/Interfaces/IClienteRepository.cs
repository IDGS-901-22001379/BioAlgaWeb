using BioAlga.Backend.Dtos;
using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface IClienteRepository
    {
        Task<(IReadOnlyList<Cliente> Items, int Total)> BuscarAsync(ClienteQueryParams query);
        Task<Cliente?> GetByIdAsync(int id);
        Task<Cliente> AddAsync(Cliente cliente);
        Task<bool> UpdateAsync(Cliente cliente);
        Task<bool> DeleteAsync(int id);

        // Auxiliares
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }
}
