using BioAlga.Backend.Models;

namespace BioAlga.Backend.Repositories.Interfaces
{
    public interface ICajaRepository
    {
        Task<IReadOnlyList<Caja>> GetAllAsync();
        Task<Caja?> GetByIdAsync(int id);
        Task<Caja> AddAsync(Caja caja);
        Task<bool> UpdateAsync(Caja caja);
        Task<bool> DeleteAsync(int id);
        Task<Caja?> GetByNombreAsync(string nombre);

        // Auxiliar
        Task<bool> NombreExistsAsync(string nombre, int? excludeId = null);
    }
}
